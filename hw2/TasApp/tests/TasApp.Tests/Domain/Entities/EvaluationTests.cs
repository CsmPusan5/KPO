using FluentAssertions;
using TasApp.Domain.Entities;
using TasApp.Domain.Exceptions;
using TasApp.Domain.ValueObjects;

namespace TasApp.Tests.Domain.Entities;

public sealed class EvaluationTests
{
    private static Criterion BuildCriterion(decimal maxScore = 10m)
    {
        var rubricator = new Rubricator();
        return rubricator.AddCriterion(
            new CriterionName("ДЗ"),
            new MaxScore(maxScore),
            new Weight(0.4m));
    }

    [Fact]
    public void Create_WithValidData_ShouldGenerateId()
    {
        // Arrange
        var criterion = BuildCriterion();
        var studentId = StudentId.New();
        var score = new Score(5m);
        var at = DateTimeOffset.UtcNow;

        // Act
        var evaluation = Evaluation.Create(studentId, criterion, score, at);

        // Assert
        evaluation.Id.Value.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_WithScoreEqualToMax_ShouldAcceptIt()
    {
        // Arrange
        var criterion = BuildCriterion(10m);
        var studentId = StudentId.New();
        var score = new Score(10m);
        var at = DateTimeOffset.UtcNow;

        // Act
        var evaluation = Evaluation.Create(studentId, criterion, score, at);

        // Assert
        evaluation.Score.Value.Should().Be(10m);
    }

    [Fact]
    public void Create_WithScoreExceedingMax_ShouldThrowScoreOutOfRange()
    {
        // Arrange
        var criterion = BuildCriterion(10m);
        var studentId = StudentId.New();
        var score = new Score(12m);
        var at = DateTimeOffset.UtcNow;

        // Act
        Action act = () => Evaluation.Create(studentId, criterion, score, at);

        // Assert
        act.Should().Throw<ScoreOutOfRangeException>();
    }

    [Fact]
    public void Create_Call_ShouldCopyCriterionId()
    {
        // Arrange
        var criterion = BuildCriterion();
        var studentId = StudentId.New();
        var score = new Score(5m);
        var at = DateTimeOffset.UtcNow;

        // Act
        var evaluation = Evaluation.Create(studentId, criterion, score, at);

        // Assert
        evaluation.CriterionId.Should().Be(criterion.Id);
    }

    [Fact]
    public void Create_WithWhitespaceComment_ShouldNormalizeToNull()
    {
        // Arrange
        var criterion = BuildCriterion();
        var studentId = StudentId.New();
        var score = new Score(5m);
        var at = DateTimeOffset.UtcNow;

        // Act
        var evaluation = Evaluation.Create(studentId, criterion, score, at, "   ");

        // Assert
        evaluation.Comment.Should().BeNull();
    }

    [Fact]
    public void Create_WithSurroundingWhitespaceInComment_ShouldTrimIt()
    {
        // Arrange
        var criterion = BuildCriterion();
        var studentId = StudentId.New();
        var score = new Score(5m);
        var at = DateTimeOffset.UtcNow;

        // Act
        var evaluation = Evaluation.Create(studentId, criterion, score, at, "  ok  ");

        // Assert
        evaluation.Comment.Should().Be("ok");
    }

    [Fact]
    public void ChangeScore_WithValidScore_ShouldUpdate()
    {
        // Arrange
        var criterion = BuildCriterion(10m);
        var evaluation = Evaluation.Create(
            StudentId.New(), criterion, new Score(5m), DateTimeOffset.UtcNow);
        var newScore = new Score(8m);

        // Act
        evaluation.ChangeScore(newScore, criterion.MaxScore);

        // Assert
        evaluation.Score.Value.Should().Be(8m);
    }

    [Fact]
    public void ChangeScore_WithScoreExceedingMax_ShouldThrowScoreOutOfRange()
    {
        // Arrange
        var criterion = BuildCriterion(10m);
        var evaluation = Evaluation.Create(
            StudentId.New(), criterion, new Score(5m), DateTimeOffset.UtcNow);
        var newScore = new Score(11m);

        // Act
        Action act = () => evaluation.ChangeScore(newScore, criterion.MaxScore);

        // Assert
        act.Should().Throw<ScoreOutOfRangeException>();
    }

    [Fact]
    public void ChangeComment_WithWhitespace_ShouldNormalizeToNull()
    {
        // Arrange
        var criterion = BuildCriterion();
        var evaluation = Evaluation.Create(
            StudentId.New(), criterion, new Score(5m), DateTimeOffset.UtcNow, "ok");

        // Act
        evaluation.ChangeComment("  ");

        // Assert
        evaluation.Comment.Should().BeNull();
    }

    [Fact]
    public void Rehydrate_Call_ShouldPreserveAllFields()
    {
        // Arrange
        var id = EvaluationId.New();
        var studentId = StudentId.New();
        var criterionId = CriterionId.New();
        var score = new Score(7m);
        var at = DateTimeOffset.UtcNow;
        const string comment = "comment";

        // Act
        var evaluation = Evaluation.Rehydrate(id, studentId, criterionId, score, at, comment);

        // Assert
        evaluation.Id.Should().Be(id);
        evaluation.StudentId.Should().Be(studentId);
        evaluation.CriterionId.Should().Be(criterionId);
        evaluation.Score.Value.Should().Be(7m);
        evaluation.AssignedAt.Should().Be(at);
        evaluation.Comment.Should().Be("comment");
    }
}