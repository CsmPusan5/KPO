using FluentAssertions;
using NSubstitute;
using TasApp.Application.UseCases;
using TasApp.Domain.Entities;
using TasApp.Domain.Ports;
using TasApp.Domain.ValueObjects;

namespace TasApp.Tests.Application.UseCases;

public sealed class ListEvaluationsUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithEvaluations_ShouldReturnMappedDtos()
    {
        // Arrange
        var rubricator = new Rubricator();
        var criterion = rubricator.AddCriterion(
            new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m));
        var studentId = StudentId.New();
        var at = DateTimeOffset.UtcNow;
        var evaluation1 = Evaluation.Create(studentId, criterion, new Score(5m), at, "ok");
        var evaluation2 = Evaluation.Create(studentId, criterion, new Score(8m), at);

        var evaluations = Substitute.For<IEvaluationRepository>();
        evaluations.ListByStudentAsync(Arg.Any<StudentId>(), Arg.Any<CancellationToken>())
            .Returns(new List<Evaluation> { evaluation1, evaluation2 });

        var sut = new ListEvaluationsUseCase(evaluations);

        // Act
        var result = await sut.ExecuteAsync(studentId.Value);

        // Assert
        result.Should().HaveCount(2);
        result[0].Score.Should().Be(5m);
        result[0].Comment.Should().Be("ok");
        result[1].Score.Should().Be(8m);
        result[0].StudentId.Should().Be(studentId.Value);
    }

    [Fact]
    public async Task ExecuteAsync_WithNoEvaluations_ShouldReturnEmptyList()
    {
        // Arrange
        var evaluations = Substitute.For<IEvaluationRepository>();
        evaluations.ListByStudentAsync(Arg.Any<StudentId>(), Arg.Any<CancellationToken>())
            .Returns(new List<Evaluation>());

        var sut = new ListEvaluationsUseCase(evaluations);

        // Act
        var result = await sut.ExecuteAsync(Guid.NewGuid());

        // Assert
        result.Should().BeEmpty();
    }
}