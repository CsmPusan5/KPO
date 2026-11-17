using TasApp.Application.UseCases;
using TasApp.Application.Commands;
using TasApp.Application.Dtos;
using TasApp.Application.Queries;

namespace TasApp.Application.Facades;

// GoF Facade: консоль работает только с этим классом.
// Use cases, репозитории и маппинг DTO скрыты.
public sealed class TasFacade
{
    private readonly AddStudentUseCase _addStudent;
    private readonly AddCriterionUseCase _addCriterion;
    private readonly ListStudentsUseCase _listStudents;
    private readonly ListCriteriaUseCase _listCriteria;
    private readonly AssignEvaluationUseCase _assignEvaluation;
    private readonly UpdateEvaluationUseCase _updateEvaluation;
    private readonly DeleteEvaluationUseCase _deleteEvaluation;
    private readonly ListEvaluationsUseCase _listEvaluations;
    private readonly CalculateFinalScoreUseCase _calculateFinalScore;
    private readonly ExportFinalScoreUseCase _exportFinalScore;
    private readonly RenameStudentUseCase _renameStudent;
    private readonly UpdateCriterionUseCase _updateCriterion;
    private readonly RemoveCriterionUseCase _removeCriterion;
    private readonly ExportAllFinalScoresUseCase _exportAll;
    private readonly SetupRubricatorUseCase _setupRubricator;

    public TasFacade(
        AddStudentUseCase addStudent,
        AddCriterionUseCase addCriterion,
        ListStudentsUseCase listStudents,
        ListCriteriaUseCase listCriteria,
        AssignEvaluationUseCase assignEvaluation,
        UpdateEvaluationUseCase updateEvaluation,
        DeleteEvaluationUseCase deleteEvaluation,
        ListEvaluationsUseCase listEvaluations,
        CalculateFinalScoreUseCase calculateFinalScore,
        ExportFinalScoreUseCase exportFinalScore,
        RenameStudentUseCase renameStudentUseCase,
        UpdateCriterionUseCase updateCriterionUseCase,
        RemoveCriterionUseCase removeCriterionUseCase,
        ExportAllFinalScoresUseCase exportAllFinalScoresUseCase,
        SetupRubricatorUseCase setupRubricatorUseCase)
    {
        _addStudent = addStudent;
        _addCriterion = addCriterion;
        _listStudents = listStudents;
        _listCriteria = listCriteria;
        _assignEvaluation = assignEvaluation;
        _updateEvaluation = updateEvaluation;
        _deleteEvaluation = deleteEvaluation;
        _listEvaluations = listEvaluations;
        _calculateFinalScore = calculateFinalScore;
        _exportFinalScore = exportFinalScore;
        _renameStudent = renameStudentUseCase;
        _updateCriterion = updateCriterionUseCase;
        _removeCriterion = removeCriterionUseCase;
        _exportAll = exportAllFinalScoresUseCase;
        _setupRubricator = setupRubricatorUseCase;
    }

    // Наружу отдаётся Guid, а не доменный VO: Facade - граница Application-слоя.
    public async Task<Guid> AddStudentAsync(string name, CancellationToken ct = default)
    {
        var id = await _addStudent.ExecuteAsync(new AddStudentCommand(name), ct);
        return id.Value;
    }

    public async Task<Guid> AddCriterionAsync(string name, decimal maxScore, decimal weight, CancellationToken ct = default)
    {
        var id = await _addCriterion.ExecuteAsync(new AddCriterionCommand(name, maxScore, weight), ct);
        return id.Value;
    }

    public Task<IReadOnlyList<StudentDto>> ListStudentsAsync(CancellationToken ct = default)
        => _listStudents.ExecuteAsync(ct);

    public Task<IReadOnlyList<CriterionDto>> ListCriteriaAsync(CancellationToken ct = default)
        => _listCriteria.ExecuteAsync(ct);

    public async Task<Guid> AssignEvaluationAsync(Guid studentId, Guid criterionId, decimal score, string? comment, CancellationToken ct = default)
    {
        var cmd = new AssignEvaluationCommand(studentId, criterionId, score, comment);
        var id = await _assignEvaluation.ExecuteAsync(cmd, ct);
        return id.Value;
    }

    public Task UpdateEvaluationAsync(Guid evaluationId, decimal? newScore, string? comment, bool changeComment, CancellationToken ct = default)
        => _updateEvaluation.ExecuteAsync(new UpdateEvaluationCommand(evaluationId, newScore, comment, changeComment), ct);

    public Task DeleteEvaluationAsync(Guid evaluationId, CancellationToken ct = default)
        => _deleteEvaluation.ExecuteAsync(evaluationId, ct);

    public Task<IReadOnlyList<EvaluationDto>> ListEvaluationsAsync(Guid studentId, CancellationToken ct = default)
        => _listEvaluations.ExecuteAsync(studentId, ct);

    public Task<FinalScoreDto> CalculateFinalScoreAsync(Guid studentId, CancellationToken ct = default)
        => _calculateFinalScore.ExecuteAsync(new CalculateFinalScoreQuery(studentId), ct);

    public Task ExportFinalScoreAsync(Guid studentId, string filePath, CancellationToken ct = default)
        => _exportFinalScore.ExecuteAsync(new ExportFinalScoreCommand(studentId, filePath), ct);
    
    public Task RenameStudentAsync(Guid studentId, string newName, CancellationToken ct = default)
       => _renameStudent.ExecuteAsync(new RenameStudentCommand(studentId, newName), ct);

    public Task UpdateCriterionAsync(Guid criterionId, string newName, decimal newMax, decimal newWeight, CancellationToken ct = default)
        => _updateCriterion.ExecuteAsync(new UpdateCriterionCommand(criterionId, newName, newMax, newWeight), ct);

    public Task RemoveCriterionAsync(Guid criterionId, CancellationToken ct = default)
        => _removeCriterion.ExecuteAsync(new RemoveCriterionCommand(criterionId), ct);
    
    public Task<int> ExportAllFinalScoresAsync(string filePath, CancellationToken ct = default)
      => _exportAll.ExecuteAsync(filePath, ct);

    public Task<IReadOnlyList<CriterionDto>> SetupRubricatorAsync(
        IReadOnlyList<SetupCriterionCommand> criteria,
        CancellationToken ct = default)
        => _setupRubricator.ExecuteAsync(new SetupRubricatorCommand(criteria), ct);
}