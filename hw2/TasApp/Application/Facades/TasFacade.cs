using TasApp.Application.UseCases;
using TasApp.Application.Commands;
using TasApp.Application.Dtos;
using TasApp.Application.Queries;

namespace TasApp.Application.Facades;

/// <summary>
/// GoF Facade: единая точка сценариев для консоли.
/// Консоль не знает про use cases, репозитории и DTO-маппинг.
/// </summary>
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
        ExportFinalScoreUseCase exportFinalScore)
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
    }

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

    public Task UpdateEvaluationAsync(Guid evaluationId, decimal newScore, string? comment, CancellationToken ct = default)
        => _updateEvaluation.ExecuteAsync(new UpdateEvaluationCommand(evaluationId, newScore, comment), ct);

    public Task DeleteEvaluationAsync(Guid evaluationId, CancellationToken ct = default)
        => _deleteEvaluation.ExecuteAsync(evaluationId, ct);

    public Task<IReadOnlyList<EvaluationDto>> ListEvaluationsAsync(Guid studentId, CancellationToken ct = default)
        => _listEvaluations.ExecuteAsync(studentId, ct);

    public Task<FinalScoreDto> CalculateFinalScoreAsync(Guid studentId, CancellationToken ct = default)
        => _calculateFinalScore.ExecuteAsync(new CalculateFinalScoreQuery(studentId), ct);

    public Task ExportFinalScoreAsync(Guid studentId, string filePath, CancellationToken ct = default)
        => _exportFinalScore.ExecuteAsync(new ExportFinalScoreCommand(studentId, filePath), ct);
}