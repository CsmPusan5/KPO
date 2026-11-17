using FluentAssertions;
using TasApp.Domain.Entities;
using TasApp.Domain.Services;
using TasApp.Domain.ValueObjects;

namespace TasApp.Tests.Domain.Services;

public sealed class PercentStrategyTests
{
    [Fact]
    public void Returns_100_When_All_Max()
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
        var sut = new PercentStrategy();

        // Act
        var result = sut.Calculate(evaluations, rubricator);

        // Assert
        result.Value.Should().Be(100m);
    }

    [Fact]
    public void Returns_Zero_When_No_Evaluations()
    {
        // Arrange
        var rubricator = new Rubricator();
        rubricator.AddCriterion(new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m));
        var sut = new PercentStrategy();

        // Act
        var result = sut.Calculate(Array.Empty<Evaluation>(), rubricator);

        // Assert
        result.Value.Should().Be(0m);
    }

    [Fact]
    public void Returns_Zero_When_Rubricator_Empty()
    {
        // Arrange
        var sut = new PercentStrategy();

        // Act
        var result = sut.Calculate(Array.Empty<Evaluation>(), new Rubricator());

        // Assert
        result.Value.Should().Be(0m);
    }

    [Fact]
    public void Calculates_Weighted_Percent()
    {
        // Arrange
        var rubricator = new Rubricator();
        var c1 = rubricator.AddCriterion(new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m));
        var c2 = rubricator.AddCriterion(new CriterionName("Экзамен"), new MaxScore(10m), new Weight(0.6m));
        var studentId = StudentId.New();
        var evaluations = new List<Evaluation>
        {
            Evaluation.Create(studentId, c1, new Score(5m), DateTimeOffset.UtcNow),
            Evaluation.Create(studentId, c2, new Score(10m), DateTimeOffset.UtcNow),
        };
        var sut = new PercentStrategy();

        // Act
        var result = sut.Calculate(evaluations, rubricator);

        // Assert: (5*0.4 + 10*0.6) / (10*0.4 + 10*0.6) * 100 = 80
        result.Value.Should().Be(80m);
    }
}