using FluentAssertions;
using NSubstitute;
using TasApp.Application.Queries;
using TasApp.Application.UseCases;
using TasApp.Domain.Entities;
using TasApp.Domain.Ports;
using TasApp.Domain.Services;
using TasApp.Domain.ValueObjects;

namespace TasApp.Tests.Application.UseCases;

public sealed class CalculateFinalScoreUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithEvaluations_ShouldReturnCalculatedScore()
    {
        // Arrange
        var rubricator = new Rubricator();
        var c1 = rubricator.AddCriterion(new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m));
        var c2 = rubricator.AddCriterion(new CriterionName("Экзамен"), new MaxScore(10m), new Weight(0.6m));
        var studentId = StudentId.New();
        var evaluations = new List<Evaluation>
        {
            Evaluation.Create(studentId, c1, new Score(8m), DateTimeOffset.UtcNow),
            Evaluation.Create(studentId, c2, new Score(9m), DateTimeOffset.UtcNow),
        };

        var evalRepo = Substitute.For<IEvaluationRepository>();
        var rubRepo = Substitute.For<IRubricatorRepository>();
        evalRepo.ListByStudentAsync(Arg.Any<StudentId>(), Arg.Any<CancellationToken>())
            .Returns(evaluations);
        rubRepo.GetAsync(Arg.Any<CancellationToken>()).Returns(rubricator);

        var calculator = new FinalScoreCalculator(new WeightedSumStrategy());
        var sut = new CalculateFinalScoreUseCase(evalRepo, rubRepo, calculator);

        // Act
        var result = await sut.ExecuteAsync(new CalculateFinalScoreQuery(studentId.Value));

        // Assert
        result.Value.Should().Be(0.86m);
        result.StudentId.Should().Be(studentId.Value);
        result.Strategy.Should().Be("WeightedSum");
    }

    [Fact]
    public async Task ExecuteAsync_WithNoEvaluations_ShouldReturnZero()
    {
        // Arrange
        var rubricator = new Rubricator();
        rubricator.AddCriterion(new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m));

        var evalRepo = Substitute.For<IEvaluationRepository>();
        var rubRepo = Substitute.For<IRubricatorRepository>();
        evalRepo.ListByStudentAsync(Arg.Any<StudentId>(), Arg.Any<CancellationToken>())
            .Returns(new List<Evaluation>());
        rubRepo.GetAsync(Arg.Any<CancellationToken>()).Returns(rubricator);

        var calculator = new FinalScoreCalculator(new WeightedSumStrategy());
        var sut = new CalculateFinalScoreUseCase(evalRepo, rubRepo, calculator);

        // Act
        var result = await sut.ExecuteAsync(new CalculateFinalScoreQuery(Guid.NewGuid()));

        // Assert
        result.Value.Should().Be(0m);
    }
}