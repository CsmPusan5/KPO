using FluentAssertions;
using NSubstitute;
using TasApp.Application.Exceptions;
using TasApp.Application.UseCases;
using TasApp.Domain.Entities;
using TasApp.Domain.Ports;
using TasApp.Domain.ValueObjects;

namespace TasApp.Tests.Application.UseCases;

public sealed class DeleteEvaluationUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithExistingEvaluation_ShouldDelete()
    {
        // Arrange
        var rubricator = new Rubricator();
        var criterion = rubricator.AddCriterion(
            new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m));
        var evaluation = Evaluation.Create(
            StudentId.New(), criterion, new Score(5m), DateTimeOffset.UtcNow);

        var evaluations = Substitute.For<IEvaluationRepository>();
        evaluations.GetByIdAsync(Arg.Any<EvaluationId>(), Arg.Any<CancellationToken>())
            .Returns(evaluation);

        var sut = new DeleteEvaluationUseCase(evaluations);

        // Act
        await sut.ExecuteAsync(evaluation.Id.Value);

        // Assert
        await evaluations.Received(1).DeleteAsync(
            Arg.Is<EvaluationId>(id => id == evaluation.Id),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithMissingEvaluation_ShouldThrowEvaluationNotFound()
    {
        // Arrange
        var evaluations = Substitute.For<IEvaluationRepository>();
        evaluations.GetByIdAsync(Arg.Any<EvaluationId>(), Arg.Any<CancellationToken>())
            .Returns((Evaluation?)null);

        var sut = new DeleteEvaluationUseCase(evaluations);

        // Act
        Func<Task> act = async () => await sut.ExecuteAsync(Guid.NewGuid());

        // Assert
        await act.Should().ThrowAsync<EvaluationNotFoundException>();
        await evaluations.DidNotReceive().DeleteAsync(Arg.Any<EvaluationId>(), Arg.Any<CancellationToken>());
    }
}