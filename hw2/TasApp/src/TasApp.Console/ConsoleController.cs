using TasApp.Application.Commands;
using TasApp.Application.Dtos;
using TasApp.Application.Exceptions;
using TasApp.Application.Facades;
using TasApp.Domain.Exceptions;

namespace TasApp.Console;

public sealed class ConsoleController
{
    private readonly TasFacade _facade;

    public ConsoleController(TasFacade facade)
        => _facade = facade ?? throw new ArgumentNullException(nameof(facade));

    public async Task RunAsync(string[] args)
    {
        if (args.Length > 0 && args[0] == "demo")
        {
            await RunDemoAsync();
            return;
        }

        await RunMenuAsync();
    }

    private async Task RunMenuAsync()
    {
        while (true)
        {
            PrintMenu();
            System.Console.Write("> ");
            var choice = (System.Console.ReadLine() ?? string.Empty).Trim();

            try
            {
                switch (choice)
                {
                    case "1": await AddStudentAsync(); break;
                    case "2": await RenameStudentAsync(); break;
                    case "3": await AddCriterionAsync(); break;
                    case "4": await UpdateCriterionAsync(); break;
                    case "5": await RemoveCriterionAsync(); break;
                    case "6": await SetupRubricatorAsync(); break;
                    case "7": await ListStudentsAsync(); break;
                    case "8": await ListCriteriaAsync(); break;
                    case "9": await AssignEvaluationAsync(); break;
                    case "10": await UpdateEvaluationAsync(); break;
                    case "11": await DeleteEvaluationAsync(); break;
                    case "12": await ListEvaluationsAsync(); break;
                    case "13": await CalculateFinalScoreAsync(); break;
                    case "14": await ExportFinalScoreAsync(); break;
                    case "15": await ExportAllFinalScoresAsync(); break;
                    case "16": await RunDemoAsync(); break;
                    case "0": return;
                    default: WriteError("Неизвестная команда"); break;
                }
            }
            catch (DomainException ex)
            {
                WriteError($"Нарушено правило: {ex.Message}");
            }
            catch (TasApplicationException ex)
            {
                WriteError($"Ошибка сценария: {ex.Message}");
            }
            catch (Exception ex)
            {
                WriteError($"Непредвиденная ошибка: {ex.Message}");
            }
        }
    }

    private static void PrintMenu()
    {
        System.Console.WriteLine("1.  Добавить студента");
        System.Console.WriteLine("2.  Переименовать студента");
        System.Console.WriteLine("3.  Добавить критерий");
        System.Console.WriteLine("4.  Изменить критерий");
        System.Console.WriteLine("5.  Удалить критерий");
        System.Console.WriteLine("6.  Настроить рубрикатор (заменить целиком)");
        System.Console.WriteLine("7.  Список студентов");
        System.Console.WriteLine("8.  Список критериев");
        System.Console.WriteLine("9.  Выставить оценку");
        System.Console.WriteLine("10. Исправить оценку");
        System.Console.WriteLine("11. Отменить оценку");
        System.Console.WriteLine("12. Оценки студента");
        System.Console.WriteLine("13. Итог студента");
        System.Console.WriteLine("14. Экспорт итога в CSV");
        System.Console.WriteLine("15. Экспорт итогов всех студентов");
        System.Console.WriteLine("16. Демо-сценарий");
        System.Console.WriteLine("0.  Выход");
    }

    // --- Команды ---

    private async Task AddStudentAsync()
    {
        var name = ReadOptional("Имя студента");
        if (name is null) { PrintCancelled(); return; }

        await _facade.AddStudentAsync(name);
        System.Console.WriteLine($"Создан студент: {name}");
    }

    private async Task AddCriterionAsync()
    {
        var name = ReadOptional("Название критерия");
        if (name is null) { PrintCancelled(); return; }

        var max = ReadOptionalDecimal("Максимальный балл");
        if (max is null) { PrintCancelled(); return; }
        if (max.Value <= 0) { WriteError("Максимальный балл должен быть больше нуля"); return; }

        var weight = ReadOptionalDecimal("Вес");
        if (weight is null) { PrintCancelled(); return; }
        if (weight.Value <= 0) { WriteError("Вес должен быть больше нуля"); return; }

        await _facade.AddCriterionAsync(name, max.Value, weight.Value);
        System.Console.WriteLine($"Создан критерий: {name} (max={max}, weight={weight})");
    }

    private async Task ListStudentsAsync()
    {
        var students = await _facade.ListStudentsAsync();
        if (students.Count == 0) { System.Console.WriteLine("Студентов нет"); return; }

        for (var i = 0; i < students.Count; i++)
            System.Console.WriteLine($"  {i + 1}. {students[i].Name}");
    }

    private async Task ListCriteriaAsync()
    {
        var criteria = await _facade.ListCriteriaAsync();
        if (criteria.Count == 0) { System.Console.WriteLine("Критериев нет"); return; }

        for (var i = 0; i < criteria.Count; i++)
        {
            var c = criteria[i];
            System.Console.WriteLine($"  {i + 1}. {c.Name} (max={c.MaxScore}, weight={c.Weight})");
        }
    }

    private async Task AssignEvaluationAsync()
    {
        var student = await SelectStudentAsync();
        if (student is null) return;

        var criterion = await SelectCriterionAsync();
        if (criterion is null) return;

        var score = ReadOptionalDecimal($"Балл (max {criterion.MaxScore})");
        if (score is null) { PrintCancelled(); return; }

        if (score.Value > criterion.MaxScore)
        {
            WriteError($"Балл не может превышать {criterion.MaxScore}");
            return;
        }

        var comment = ReadOptional("Комментарий (Enter — без комментария)");

        await _facade.AssignEvaluationAsync(student.Id, criterion.Id, score.Value, comment);
        System.Console.WriteLine($"Создана оценка: {student.Name} / {criterion.Name} = {score}");
    }

    private async Task UpdateEvaluationAsync()
    {
        var student = await SelectStudentAsync();
        if (student is null) return;

        var evaluation = await SelectEvaluationAsync(student.Id);
        if (evaluation is null) return;

        System.Console.WriteLine($"Текущая оценка: {evaluation.CriterionName} = {evaluation.Score}");

        var newScore = ReadOptionalDecimal("Новый балл (Enter — не менять)");

        var changeAnswer = ReadOptional("Изменить комментарий? y/n");
        var changeComment = changeAnswer is "y" or "Y";

        string? newComment = null;
        if (changeComment)
        {
            System.Console.Write("Новый комментарий (пусто — очистить): ");
            var raw = System.Console.ReadLine();
            newComment = string.IsNullOrWhiteSpace(raw) ? null : raw.Trim();
        }

        if (newScore is null && !changeComment)
        {
            System.Console.WriteLine("Ничего не изменено");
            return;
        }

        await _facade.UpdateEvaluationAsync(evaluation.Id, newScore, newComment, changeComment);

        var scoreText = newScore is { } s ? s.ToString("0.##") : evaluation.Score.ToString("0.##");
        System.Console.WriteLine($"Оценка обновлена: {evaluation.CriterionName} = {scoreText}");
    }

    private async Task DeleteEvaluationAsync()
    {
        var student = await SelectStudentAsync();
        if (student is null) return;

        var evaluation = await SelectEvaluationAsync(student.Id);
        if (evaluation is null) return;

        await _facade.DeleteEvaluationAsync(evaluation.Id);
        System.Console.WriteLine($"Оценка отменена: {student.Name} / {evaluation.CriterionName}");
    }

    private async Task ListEvaluationsAsync()
    {
        var student = await SelectStudentAsync();
        if (student is null) return;

        var evaluations = await _facade.ListEvaluationsAsync(student.Id);
        if (evaluations.Count == 0) { System.Console.WriteLine("Оценок нет"); return; }

        System.Console.WriteLine($"Оценки студента {student.Name}:");
        for (var i = 0; i < evaluations.Count; i++)
        {
            var e = evaluations[i];
            var comment = string.IsNullOrEmpty(e.Comment) ? string.Empty : $"  // {e.Comment}";
            System.Console.WriteLine($"  {i + 1}. {e.CriterionName}: {e.Score}{comment}");
        }
    }

    private async Task CalculateFinalScoreAsync()
    {
        var student = await SelectStudentAsync();
        if (student is null) return;

        var dto = await _facade.CalculateFinalScoreAsync(student.Id);
        System.Console.WriteLine($"{student.Name}: {dto.Value:0.##}  (стратегия: {dto.Strategy})");
    }

    private async Task ExportFinalScoreAsync()
    {
        var student = await SelectStudentAsync();
        if (student is null) return;

        var path = ReadOptional("Путь к файлу (например, out/report.csv)");
        if (path is null) { PrintCancelled(); return; }

        var dto = await _facade.CalculateFinalScoreAsync(student.Id);
        await _facade.ExportFinalScoreAsync(student.Id, path);

        System.Console.WriteLine($"Экспортирован итог: {student.Name} = {dto.Value:0.##} → {path}");
    }

    // --- Демо ---

    private async Task RunDemoAsync()
    {
        System.Console.WriteLine("=== Демо ===");

        var studentA = await _facade.AddStudentAsync("Иван");
        var studentB = await _facade.AddStudentAsync("Пётр");
        System.Console.WriteLine("Студенты: Иван, Пётр");

        var criteria = await _facade.SetupRubricatorAsync(
        [
            new SetupCriterionCommand("ДЗ", 10m, 0.4m),
            new SetupCriterionCommand("Экзамен", 10m, 0.6m),
        ]);
        var criterionHw = criteria.First(c => c.Name == "ДЗ").Id;
        var criterionExam = criteria.First(c => c.Name == "Экзамен").Id;
        System.Console.WriteLine("Критерии: ДЗ (max=10, w=0.4), Экзамен (max=10, w=0.6)");

        await _facade.AssignEvaluationAsync(studentA, criterionHw, 8m, "хорошо");
        await _facade.AssignEvaluationAsync(studentA, criterionExam, 9m, null);
        await _facade.AssignEvaluationAsync(studentB, criterionHw, 6m, null);
        await _facade.AssignEvaluationAsync(studentB, criterionExam, 7m, null);
        System.Console.WriteLine("Оценки выставлены");

        var scoreA = await _facade.CalculateFinalScoreAsync(studentA);
        var scoreB = await _facade.CalculateFinalScoreAsync(studentB);
        System.Console.WriteLine($"Итог Ивана: {scoreA.Value:0.##}");
        System.Console.WriteLine($"Итог Петра: {scoreB.Value:0.##}");

        await _facade.ExportAllFinalScoresAsync("data/demo/all.csv");
        System.Console.WriteLine("Итоги всех студентов экспортированы в data/demo/all.csv");

        System.Console.WriteLine("=== Демо завершено ===");
    }

    // --- Select helpers ---

    private async Task<StudentDto?> SelectStudentAsync()
    {
        var students = await _facade.ListStudentsAsync();
        if (students.Count == 0) { System.Console.WriteLine("Студентов нет"); return null; }

        System.Console.WriteLine("Выберите студента:");
        for (var i = 0; i < students.Count; i++)
            System.Console.WriteLine($"  {i + 1}. {students[i].Name}");

        var idx = ReadOptionalInt("Номер", 1, students.Count);
        if (idx is null) { PrintCancelled(); return null; }

        return students[idx.Value - 1];
    }

    private async Task<CriterionDto?> SelectCriterionAsync()
    {
        var criteria = await _facade.ListCriteriaAsync();
        if (criteria.Count == 0) { System.Console.WriteLine("Критериев нет"); return null; }

        System.Console.WriteLine("Выберите критерий:");
        for (var i = 0; i < criteria.Count; i++)
        {
            var c = criteria[i];
            System.Console.WriteLine($"  {i + 1}. {c.Name} (max={c.MaxScore})");
        }

        var idx = ReadOptionalInt("Номер", 1, criteria.Count);
        if (idx is null) { PrintCancelled(); return null; }

        return criteria[idx.Value - 1];
    }

    private async Task<EvaluationDto?> SelectEvaluationAsync(Guid studentId)
    {
        var evaluations = await _facade.ListEvaluationsAsync(studentId);
        if (evaluations.Count == 0) { System.Console.WriteLine("Оценок нет"); return null; }

        System.Console.WriteLine("Выберите оценку:");
        for (var i = 0; i < evaluations.Count; i++)
        {
            var e = evaluations[i];
            var comment = string.IsNullOrEmpty(e.Comment) ? string.Empty : $"  // {e.Comment}";
            System.Console.WriteLine($"  {i + 1}. {e.CriterionName}: {e.Score}{comment}");
        }

        var idx = ReadOptionalInt("Номер", 1, evaluations.Count);
        if (idx is null) { PrintCancelled(); return null; }

        return evaluations[idx.Value - 1];
    }

    // --- Ввод ---

    private static string? ReadOptional(string prompt)
    {
        System.Console.Write($"{prompt} (Enter — отмена): ");
        var input = System.Console.ReadLine();
        return string.IsNullOrWhiteSpace(input) ? null : input.Trim();
    }

    private static decimal? ReadOptionalDecimal(string prompt)
    {
        while (true)
        {
            var raw = ReadOptional(prompt);
            if (raw is null) return null;

            if (decimal.TryParse(raw,
                    System.Globalization.NumberStyles.AllowDecimalPoint
                    | System.Globalization.NumberStyles.AllowLeadingSign,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var value))
            {
                if (value < 0)
                {
                    WriteError("Число не может быть отрицательным");
                    continue;
                }
                return value;
            }

            WriteError("Ожидается число (например, 8 или 8.5)");
        }
    }

    private static int? ReadOptionalInt(string prompt, int min, int max)
    {
        while (true)
        {
            var raw = ReadOptional($"{prompt} ({min}-{max})");
            if (raw is null) return null;

            if (int.TryParse(raw, out var value) && value >= min && value <= max)
                return value;

            WriteError($"Введите число от {min} до {max}");
        }
    }

    // --- Вывод ---

    private static void PrintCancelled() => System.Console.WriteLine("Отменено");

    private static void WriteError(string message)
    {
        var prev = System.Console.ForegroundColor;
        System.Console.ForegroundColor = ConsoleColor.Red;
        System.Console.WriteLine(message);
        System.Console.ForegroundColor = prev;
    }

    private async Task RenameStudentAsync()
    {
        var student = await SelectStudentAsync();
        if (student is null) return;

        var newName = ReadOptional("Новое имя");
        if (newName is null) { PrintCancelled(); return; }

        await _facade.RenameStudentAsync(student.Id, newName);
        System.Console.WriteLine($"Студент переименован: {newName}");
    }

    private async Task UpdateCriterionAsync()
    {
        var criterion = await SelectCriterionAsync();
        if (criterion is null) return;

        var newName = ReadOptional("Новое название");
        if (newName is null) { PrintCancelled(); return; }

        var newMax = ReadOptionalDecimal("Новый максимальный балл");
        if (newMax is null) { PrintCancelled(); return; }
        if (newMax.Value <= 0) { WriteError("Максимальный балл должен быть > 0"); return; }

        var newWeight = ReadOptionalDecimal("Новый вес");
        if (newWeight is null) { PrintCancelled(); return; }
        if (newWeight.Value <= 0) { WriteError("Вес должен быть > 0"); return; }

        await _facade.UpdateCriterionAsync(criterion.Id, newName, newMax.Value, newWeight.Value);
        System.Console.WriteLine($"Критерий обновлён: {newName}");
    }

    private async Task RemoveCriterionAsync()
    {
        var criterion = await SelectCriterionAsync();
        if (criterion is null) return;

        var confirm = ReadOptional($"Удалить критерий «{criterion.Name}»? y/n");
        if (confirm is not ("y" or "Y")) { PrintCancelled(); return; }

        await _facade.RemoveCriterionAsync(criterion.Id);
        System.Console.WriteLine($"Критерий удалён: {criterion.Name}");
    }

    private async Task ExportAllFinalScoresAsync()
    {
        var path = ReadOptional("Путь к файлу (например, out/all.csv)");
        if (path is null) { PrintCancelled(); return; }

        var count = await _facade.ExportAllFinalScoresAsync(path);
        if (count == 0) { System.Console.WriteLine("Нет студентов для экспорта"); return; }

        System.Console.WriteLine($"Экспортировано {count} итогов → {path}");
    }

    private async Task SetupRubricatorAsync()
    {
        var existing = await _facade.ListCriteriaAsync();
        if (existing.Count > 0)
        {
            var confirm = ReadOptional($"Заменить текущий рубрикатор ({existing.Count} критериев)? y/n");
            if (confirm is not ("y" or "Y")) { PrintCancelled(); return; }
        }

        System.Console.WriteLine("Введите критерии. Пустое название — завершить ввод.");
        var drafts = new List<SetupCriterionCommand>();

        while (true)
        {
            var name = ReadOptional($"Название критерия #{drafts.Count + 1}");
            if (name is null) break;

            var max = ReadOptionalDecimal("Максимальный балл");
            if (max is null) break;
            if (max.Value <= 0) { WriteError("Балл должен быть > 0"); continue; }

            var weight = ReadOptionalDecimal("Вес");
            if (weight is null) break;
            if (weight.Value <= 0) { WriteError("Вес должен быть > 0"); continue; }

            drafts.Add(new SetupCriterionCommand(name, max.Value, weight.Value));
        }

        if (drafts.Count == 0) { System.Console.WriteLine("Рубрикатор не изменён"); return; }

        var created = await _facade.SetupRubricatorAsync(drafts);

        System.Console.WriteLine($"Рубрикатор настроен: {created.Count} критериев");
        foreach (var c in created)
            System.Console.WriteLine($"  {c.Name} (max={c.MaxScore}, w={c.Weight})");
    }
}