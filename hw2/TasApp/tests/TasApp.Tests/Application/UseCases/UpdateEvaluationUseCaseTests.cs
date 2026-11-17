using FluentAssertions;
using NSubstitute;
using TasApp.Application.Commands;
using TasApp.Application.Exceptions;
using TasApp.Application.UseCases;
using TasApp.Domain.Entities;
using TasApp.Domain.Exceptions;
using TasApp.Domain.Ports;
using TasApp.Domain.ValueObjects;

namespace TasApp.Tests.Application.UseCases;

public sealed class UpdateEvaluationUseCaseTests
{
    private static (Evaluation evaluation, Criterion criterion, Rubricator rubricator) Build()
    {
        var rubricator = new Rubricator();
        var criterion = rubricator.AddCriterion(
            new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m));
        var evaluation = Evaluation.Create(
            StudentId.New(), criterion, new Score(5m), DateTimeOffset.UtcNow, "old");
        return (evaluation, criterion, rubricator);
    }

    [Fact]
    public async Task ExecuteAsync_WithNewScoreOnly_ShouldNotChangeComment()
    {
        // Arrange
        var (evaluation, _, rubricator) = Build();

        var evaluations = Substitute.For<IEvaluationRepository>();
        var rubricators = Substitute.For<IRubricatorRepository>();
        evaluations.GetByIdAsync(Arg.Any<EvaluationId>(), Arg.Any<CancellationToken>())
            .Returns(evaluation);
        rubricators.GetAsync(Arg.Any<CancellationToken>()).Returns(rubricator);

        var sut = new UpdateEvaluationUseCase(evaluations, rubricators);
        var cmd = new UpdateEvaluationCommand(
            evaluation.Id.Value, NewScore: 8m, Comment: null, ChangeComment: false);

        // Act
        await sut.ExecuteAsync(cmd);

        // Assert
        evaluation.Score.Value.Should().Be(8m);
        evaluation.Comment.Should().Be("old");
        await evaluations.Received(1).UpdateAsync(evaluation, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithChangeCommentTrue_ShouldOverwriteComment()
    {
        // Arrange
        var (evaluation, _, rubricator) = Build();

        var evaluations = Substitute.For<IEvaluationRepository>();
        var rubricators = Substitute.For<IRubricatorRepository>();
        evaluations.GetByIdAsync(Arg.Any<EvaluationId>(), Arg.Any<CancellationToken>())
            .Returns(evaluation);
        rubricators.GetAsync(Arg.Any<CancellationToken>()).Returns(rubricator);

        var sut = new UpdateEvaluationUseCase(evaluations, rubricators);
        var cmd = new UpdateEvaluationCommand(
            evaluation.Id.Value, NewScore: null, Comment: "new", ChangeComment: true);

        // Act
        await sut.ExecuteAsync(cmd);

        // Assert
        evaluation.Score.Value.Should().Be(5m);
        evaluation.Comment.Should().Be("new");
    }

    [Fact]
    public async Task ExecuteAsync_WithScoreExceedingMax_ShouldThrowScoreOutOfRange()
    {
        // Arrange
        var (evaluation, _, rubricator) = Build();

        var evaluations = Substitute.For<IEvaluationRepository>();
        var rubricators = Substitute.For<IRubricatorRepository>();
        evaluations.GetByIdAsync(Arg.Any<EvaluationId>(), Arg.Any<CancellationToken>())
            .Returns(evaluation);
        rubricators.GetAsync(Arg.Any<CancellationToken>()).Returns(rubricator);

        var sut = new UpdateEvaluationUseCase(evaluations, rubricators);
        var cmd = new UpdateEvaluationCommand(
            evaluation.Id.Value, NewScore: 15m, Comment: null, ChangeComment: false);

        // Act
        Func<Task> act = async () => await sut.ExecuteAsync(cmd);

        // Assert
        await act.Should().ThrowAsync<ScoreOutOfRangeException>();
    }

    [Fact]
    public async Task ExecuteAsync_WithMissingEvaluation_ShouldThrowEvaluationNotFound()
    {
        // Arrange
        var evaluations = Substitute.For<IEvaluationRepository>();
        var rubricators = Substitute.For<IRubricatorRepository>();
        evaluations.GetByIdAsync(Arg.Any<EvaluationId>(), Arg.Any<CancellationToken>())
            .Returns((Evaluation?)null);

        var sut = new UpdateEvaluationUseCase(evaluations, rubricators);
        var cmd = new UpdateEvaluationCommand(
            Guid.NewGuid(), NewScore: 8m, Comment: null, ChangeComment: false);

        // Act
        Func<Task> act = async () => await sut.ExecuteAsync(cmd);

        // Assert
        await act.Should().ThrowAsync<EvaluationNotFoundException>();
        await evaluations.DidNotReceive()
            .UpdateAsync(Arg.Any<Evaluation>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithNoChanges_ShouldStillSave()
    {
        // Arrange — оба поля null/false, но use case всё равно сохраняет
        var (evaluation, _, rubricator) = Build();

        var evaluations = Substitute.For<IEvaluationRepository>();
        var rubricators = Substitute.For<IRubricatorRepository>();
        evaluations.GetByIdAsync(Arg.Any<EvaluationId>(), Arg.Any<CancellationToken>())
            .Returns(evaluation);
        rubricators.GetAsync(Arg.Any<CancellationToken>()).Returns(rubricator);

        var sut = new UpdateEvaluationUseCase(evaluations, rubricators);
        var cmd = new UpdateEvaluationCommand(
            evaluation.Id.Value, NewScore: null, Comment: null, ChangeComment: false);

        // Act
        await sut.ExecuteAsync(cmd);

        // Assert — балл и комментарий не изменились
        evaluation.Score.Value.Should().Be(5m);
        evaluation.Comment.Should().Be("old");
    }
}