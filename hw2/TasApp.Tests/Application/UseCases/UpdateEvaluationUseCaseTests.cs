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
    [Fact]
    public async Task ExecuteAsync_WithValidScore_ShouldUpdateAndSave()
    {
        // Arrange
        var rubricator = new Rubricator();
        var criterion = rubricator.AddCriterion(
            new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m));
        var evaluation = Evaluation.Create(
            StudentId.New(), criterion, new Score(5m), DateTimeOffset.UtcNow, "old");

        var evaluations = Substitute.For<IEvaluationRepository>();
        var rubricators = Substitute.For<IRubricatorRepository>();
        evaluations.GetByIdAsync(Arg.Any<EvaluationId>(), Arg.Any<CancellationToken>())
            .Returns(evaluation);
        rubricators.GetAsync(Arg.Any<CancellationToken>()).Returns(rubricator);

        var sut = new UpdateEvaluationUseCase(evaluations, rubricators);
        var cmd = new UpdateEvaluationCommand(evaluation.Id.Value, 8m, "new");

        // Act
        await sut.ExecuteAsync(cmd);

        // Assert
        evaluation.Score.Value.Should().Be(8m);
        evaluation.Comment.Should().Be("new");
        await evaluations.Received(1).UpdateAsync(evaluation, Arg.Any<CancellationToken>());
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
        var cmd = new UpdateEvaluationCommand(Guid.NewGuid(), 8m, null);

        // Act
        Func<Task> act = async () => await sut.ExecuteAsync(cmd);

        // Assert
        await act.Should().ThrowAsync<EvaluationNotFoundException>();
        await evaluations.DidNotReceive().UpdateAsync(Arg.Any<Evaluation>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithScoreExceedingMax_ShouldThrowScoreOutOfRange()
    {
        // Arrange
        var rubricator = new Rubricator();
        var criterion = rubricator.AddCriterion(
            new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m));
        var evaluation = Evaluation.Create(
            StudentId.New(), criterion, new Score(5m), DateTimeOffset.UtcNow);

        var evaluations = Substitute.For<IEvaluationRepository>();
        var rubricators = Substitute.For<IRubricatorRepository>();
        evaluations.GetByIdAsync(Arg.Any<EvaluationId>(), Arg.Any<CancellationToken>())
            .Returns(evaluation);
        rubricators.GetAsync(Arg.Any<CancellationToken>()).Returns(rubricator);

        var sut = new UpdateEvaluationUseCase(evaluations, rubricators);
        var cmd = new UpdateEvaluationCommand(evaluation.Id.Value, 15m, null);

        // Act
        Func<Task> act = async () => await sut.ExecuteAsync(cmd);

        // Assert
        await act.Should().ThrowAsync<ScoreOutOfRangeException>();
    }
}