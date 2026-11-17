using FluentAssertions;
using TasApp.Domain.Entities;
using TasApp.Domain.Exceptions;
using TasApp.Domain.Services;
using TasApp.Domain.ValueObjects;

namespace TasApp.Tests.Domain.Services;

public sealed class StrictWeightedSumStrategyTests
{
    [Fact]
    public void Calculate_WithMissingCriterion_ShouldThrowMissingEvaluations()
    {
        // Arrange
        var rubricator = new Rubricator();
        var c1 = rubricator.AddCriterion(new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m));
        rubricator.AddCriterion(new CriterionName("Экзамен"), new MaxScore(10m), new Weight(0.6m));
        var evaluations = new List<Evaluation>
        {
            Evaluation.Create(StudentId.New(), c1, new Score(10m), DateTimeOffset.UtcNow),
        };
        var sut = new StrictWeightedSumStrategy();

        // Act
        Action act = () => sut.Calculate(evaluations, rubricator);

        // Assert
        act.Should().Throw<MissingEvaluationsException>()
            .Which.MissingCriterionIds.Should().HaveCount(1);
    }

    [Fact]
    public void Calculate_WithAllEvaluationsPresent_ShouldReturnWeightedSum()
    {
        // Arrange
        var rubricator = new Rubricator();
        var c1 = rubricator.AddCriterion(new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m));
        var c2 = rubricator.AddCriterion(new CriterionName("Экзамен"), new MaxScore(10m), new Weight(0.6m));
        var studentId = StudentId.New();
        var evaluations = new List<Evaluation>
        {
            Evaluation.Create(studentId, c1, new Score(10m), DateTimeOffset.UtcNow),
            Evaluation.Create(studentId, c2, new Score(10m), DateTimeOffset.UtcNow),
        };
        var sut = new StrictWeightedSumStrategy();

        // Act
        var result = sut.Calculate(evaluations, rubricator);

        // Assert
        result.Value.Should().Be(1.0m);
    }
}