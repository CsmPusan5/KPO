using FluentAssertions;
using TasApp.Domain.Entities;
using TasApp.Domain.ValueObjects;
using TasApp.Infrastructure.Persistence;

namespace TasApp.Tests.Infrastructure.Persistence;

public sealed class JsonEvaluationRepositoryTests : IDisposable
{
    private readonly string _filePath;
    private readonly JsonEvaluationRepository _sut;
    private readonly Criterion _criterion;

    public JsonEvaluationRepositoryTests()
    {
        _filePath = Path.Combine(Path.GetTempPath(), $"evaluations-{Guid.NewGuid():N}.json");
        _sut = new JsonEvaluationRepository(_filePath);

        var rubricator = new Rubricator();
        _criterion = rubricator.AddCriterion(
            new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m));
    }

    public void Dispose()
    {
        if (File.Exists(_filePath)) File.Delete(_filePath);
    }

    private Evaluation NewEvaluation(decimal score = 5m, string? comment = null)
    {
        return Evaluation.Create(
            StudentId.New(),
            _criterion,
            new Score(score),
            DateTimeOffset.UtcNow,
            comment);
    }

    [Fact]
    public async Task AddAsync_WithEvaluation_ShouldPersistToFile()
    {
        // Arrange
        var evaluation = NewEvaluation(7m, "ok");

        // Act
        await _sut.AddAsync(evaluation);

        // Assert
        File.Exists(_filePath).Should().BeTrue();

        var loaded = await _sut.GetByIdAsync(evaluation.Id);
        loaded.Should().NotBeNull();
        loaded!.Score.Value.Should().Be(7m);
        loaded.Comment.Should().Be("ok");
        loaded.StudentId.Should().Be(evaluation.StudentId);
        loaded.CriterionId.Should().Be(evaluation.CriterionId);
    }

    [Fact]
    public async Task GetByIdAsync_WithMissingId_ShouldReturnNull()
    {
        // Arrange
        var missingId = EvaluationId.New();

        // Act
        var result = await _sut.GetByIdAsync(missingId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ListByStudentAsync_WithNoEvaluations_ShouldReturnEmpty()
    {
        // Arrange
        var studentId = StudentId.New();

        // Act
        var result = await _sut.ListByStudentAsync(studentId);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ListByStudentAsync_ShouldFilterByStudent()
    {
        // Arrange
        var studentA = StudentId.New();
        var studentB = StudentId.New();

        await _sut.AddAsync(Evaluation.Create(studentA, _criterion, new Score(5m), DateTimeOffset.UtcNow));
        await _sut.AddAsync(Evaluation.Create(studentA, _criterion, new Score(7m), DateTimeOffset.UtcNow));
        await _sut.AddAsync(Evaluation.Create(studentB, _criterion, new Score(9m), DateTimeOffset.UtcNow));

        // Act
        var result = await _sut.ListByStudentAsync(studentA);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(e => e.StudentId == studentA);
    }

    [Fact]
    public async Task UpdateAsync_WithExistingEvaluation_ShouldPersistChanges()
    {
        // Arrange
        var evaluation = NewEvaluation(5m, "old");
        await _sut.AddAsync(evaluation);

        evaluation.ChangeScore(new Score(8m), _criterion.MaxScore);
        evaluation.ChangeComment("new");

        // Act
        await _sut.UpdateAsync(evaluation);

        // Assert
        var loaded = await _sut.GetByIdAsync(evaluation.Id);
        loaded!.Score.Value.Should().Be(8m);
        loaded.Comment.Should().Be("new");
    }

    [Fact]
    public async Task UpdateAsync_WithMissingEvaluation_ShouldThrowInvalidOperation()
    {
        // Arrange
        var evaluation = NewEvaluation();

        // Act
        Func<Task> act = async () => await _sut.UpdateAsync(evaluation);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task DeleteAsync_WithExistingId_ShouldRemoveEvaluation()
    {
        // Arrange
        var evaluation = NewEvaluation();
        await _sut.AddAsync(evaluation);

        // Act
        await _sut.DeleteAsync(evaluation.Id);

        // Assert
        var loaded = await _sut.GetByIdAsync(evaluation.Id);
        loaded.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithMissingId_ShouldNotThrow()
    {
        // Arrange
        var missingId = EvaluationId.New();

        // Act
        Func<Task> act = async () => await _sut.DeleteAsync(missingId);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task AddAsync_ThenGetById_ShouldPreserveAssignedAt()
    {
        // Arrange
        var assignedAt = new DateTimeOffset(2024, 11, 1, 12, 0, 0, TimeSpan.Zero);
        var evaluation = Evaluation.Create(
            StudentId.New(), _criterion, new Score(5m), assignedAt);

        // Act
        await _sut.AddAsync(evaluation);

        // Assert
        var loaded = await _sut.GetByIdAsync(evaluation.Id);
        loaded!.AssignedAt.Should().Be(assignedAt);
    }
}