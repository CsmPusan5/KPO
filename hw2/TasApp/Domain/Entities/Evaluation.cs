using TasApp.Domain.Exceptions;
using TasApp.Domain.ValueObjects;

namespace TasApp.Domain.Entities;

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

    // Принимает Criterion, а не CriterionId. Это гарантирует, что критерий был получен из Rubricator.
    // Здесь же проверяется score <= criterion.MaxScore.
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

    // Восстановление из хранилища (без валидации).
    public static Evaluation Rehydrate(
        EvaluationId id,
        StudentId studentId,
        CriterionId criterionId,
        Score score,
        DateTimeOffset assignedAt,
        string? comment)
        => new(id, studentId, criterionId, score, assignedAt, comment);

    // maxScore передаётся параметром: если критерий изменён, проверка идёт по актуальному значению.
    public void ChangeScore(Score newScore, MaxScore maxScore)
    {
        EnsureWithinMaxScore(newScore, maxScore);
        Score = newScore;
    }

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