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
                    case "2": await AddCriterionAsync(); break;
                    case "3": await ListStudentsAsync(); break;
                    case "4": await ListCriteriaAsync(); break;
                    case "5": await AssignEvaluationAsync(); break;
                    case "6": await UpdateEvaluationAsync(); break;
                    case "7": await DeleteEvaluationAsync(); break;
                    case "8": await ListEvaluationsAsync(); break;
                    case "9": await CalculateFinalScoreAsync(); break;
                    case "10": await ExportFinalScoreAsync(); break;
                    case "11": await RunDemoAsync(); break;
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
        System.Console.WriteLine();
        System.Console.WriteLine("=== ПОС: Преподавательская Оценка Студентов ===");
        System.Console.WriteLine("1.  Добавить студента");
        System.Console.WriteLine("2.  Добавить критерий");
        System.Console.WriteLine("3.  Список студентов");
        System.Console.WriteLine("4.  Список критериев");
        System.Console.WriteLine("5.  Выставить оценку");
        System.Console.WriteLine("6.  Исправить оценку");
        System.Console.WriteLine("7.  Отменить оценку");
        System.Console.WriteLine("8.  Оценки студента");
        System.Console.WriteLine("9.  Итог студента");
        System.Console.WriteLine("10. Экспорт итога в CSV");
        System.Console.WriteLine("11. Демо-сценарий");
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
        System.Console.WriteLine();
        System.Console.WriteLine("=== Демо: студенты и критерии -> оценки -> итог -> экспорт ===");

        var studentA = await _facade.AddStudentAsync("Иван");
        var studentB = await _facade.AddStudentAsync("Пётр");
        System.Console.WriteLine("Студенты: Иван, Пётр");

        var criterionHw = await _facade.AddCriterionAsync("ДЗ", 10m, 0.4m);
        var criterionExam = await _facade.AddCriterionAsync("Экзамен", 10m, 0.6m);
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

        await _facade.ExportFinalScoreAsync(studentA, "data/demo-ivan.csv");
        await _facade.ExportFinalScoreAsync(studentB, "data/demo-petr.csv");
        System.Console.WriteLine("Итоги экспортированы в data/demo-*.csv");

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
}