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

        var evaluations = Substitute.For<IEvaluationRepository>();
        var rubricators = Substitute.For<IRubricatorRepository>();

        evaluations.ListByStudentAsync(Arg.Any<StudentId>(), Arg.Any<CancellationToken>())
            .Returns(new List<Evaluation>
            {
                Evaluation.Create(studentId, criterion, new Score(5m), at, "ok"),
                Evaluation.Create(studentId, criterion, new Score(8m), at),
            });
        rubricators.GetAsync(Arg.Any<CancellationToken>()).Returns(rubricator);

        var sut = new ListEvaluationsUseCase(evaluations, rubricators);

        // Act
        var result = await sut.ExecuteAsync(studentId.Value);

        // Assert
        result.Should().HaveCount(2);
        result[0].Score.Should().Be(5m);
        result[0].Comment.Should().Be("ok");
        result[0].CriterionName.Should().Be("ДЗ");
        result[0].StudentId.Should().Be(studentId.Value);
        result[0].CriterionId.Should().Be(criterion.Id.Value);
        result[1].Score.Should().Be(8m);
    }

    [Fact]
    public async Task ExecuteAsync_WithNoEvaluations_ShouldReturnEmptyList()
    {
        // Arrange
        var evaluations = Substitute.For<IEvaluationRepository>();
        var rubricators = Substitute.For<IRubricatorRepository>();
        evaluations.ListByStudentAsync(Arg.Any<StudentId>(), Arg.Any<CancellationToken>())
            .Returns(new List<Evaluation>());

        var sut = new ListEvaluationsUseCase(evaluations, rubricators);

        // Act
        var result = await sut.ExecuteAsync(Guid.NewGuid());

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_WithRemovedCriterion_ShouldShowPlaceholder()
    {
        // Arrange
        var currentRubricator = new Rubricator();
        currentRubricator.AddCriterion(new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m));

        // Оценка ссылается на критерий, которого уже нет в рубрикаторе
        var removedCriterion = new Rubricator().AddCriterion(
            new CriterionName("Удалённый"), new MaxScore(10m), new Weight(0.4m));
        var studentId = StudentId.New();

        var evaluations = Substitute.For<IEvaluationRepository>();
        var rubricators = Substitute.For<IRubricatorRepository>();

        evaluations.ListByStudentAsync(Arg.Any<StudentId>(), Arg.Any<CancellationToken>())
            .Returns(new List<Evaluation>
            {
                Evaluation.Create(studentId, removedCriterion, new Score(5m), DateTimeOffset.UtcNow),
            });
        rubricators.GetAsync(Arg.Any<CancellationToken>()).Returns(currentRubricator);

        var sut = new ListEvaluationsUseCase(evaluations, rubricators);

        // Act
        var result = await sut.ExecuteAsync(studentId.Value);

        // Assert
        result[0].CriterionName.Should().Be("<удалён>");
    }
}