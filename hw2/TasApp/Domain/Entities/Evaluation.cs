using TasApp.Domain.Exceptions;
using TasApp.Domain.ValueObjects;

namespace TasApp.Domain.Entities;

/// <summary>
/// Оценка — факт «этому студенту по этому критерию поставили такой балл».
/// Aggregate Root. Ссылается на Student и Criterion по Id, не держит их объекты.
/// </summary>
public sealed class Evaluation
{
    public EvaluationId Id { get; }
    public StudentId StudentId { get; }
    public CriterionId CriterionId { get; }
    public Score Score { get; private set; }
    public DateTimeOffset AssignedAt { get; }
    public string? Comment { get; private set; }

    private Evaluation(
        EvaluationId id,
        StudentId studentId,
        CriterionId criterionId,
        Score score,
        DateTimeOffset assignedAt,
        string? comment)
    {
        Id = id;
        StudentId = studentId;
        CriterionId = criterionId;
        Score = score;
        AssignedAt = assignedAt;
        Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
    }

    /// <summary>
    /// Фабрика
    /// Проверяет инвариант: score ∈ [0, criterion.MaxScore].
    /// Критерий передаётся целиком, потому что нужен его MaxScore.
    /// </summary>
    public static Evaluation Create(
        StudentId studentId,
        Criterion criterion,
        Score score,
        DateTimeOffset assignedAt,
        string? comment = null)
    {
        EnsureWithinMaxScore(score, criterion.MaxScore);
        return new Evaluation(
            EvaluationId.New(),
            studentId,
            criterion.Id,
            score,
            assignedAt,
            comment);
    }

    /// <summary>Восстановление из хранилища — без валидации (данные уже валидны).</summary>
    public static Evaluation Rehydrate(
        EvaluationId id,
        StudentId studentId,
        CriterionId criterionId,
        Score score,
        DateTimeOffset assignedAt,
        string? comment)
        => new(id, studentId, criterionId, score, assignedAt, comment);

    /// <summary>
    /// Исправить балл. MaxScore передаётся снаружи (use case берёт его из актуального Rubricator).
    /// </summary>
    public void ChangeScore(Score newScore, MaxScore maxScore)
    {
        EnsureWithinMaxScore(newScore, maxScore);
        Score = newScore;
    }

    /// <summary>Изменить комментарий.</summary>
    public void ChangeComment(string? comment)
        => Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();

    private static void EnsureWithinMaxScore(Score score, MaxScore maxScore)
    {
        if (score.Value > maxScore.Value)
            throw ScoreOutOfRangeException.ExceedsMax(score.Value, maxScore.Value);
    }

    public override string ToString()
        => $"Evaluation({Id.Value:N}): student={StudentId.Value:N}, criterion={CriterionId.Value:N}, score={Score.Value}";
}