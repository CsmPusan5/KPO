using FluentAssertions;
using TasApp.Domain.Entities;
using TasApp.Domain.Services;
using TasApp.Domain.ValueObjects;

namespace TasApp.Tests.Domain.Services;

public sealed class WeightedSumStrategyTests
{
    private static Rubricator BuildRubricator(out Criterion c1, out Criterion c2)
    {
        var rubricator = new Rubricator();
        c1 = rubricator.AddCriterion(new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m));
        c2 = rubricator.AddCriterion(new CriterionName("Экзамен"), new MaxScore(10m), new Weight(0.6m));
        return rubricator;
    }

    [Fact]
    public void Calculate_WithAllEvaluationsPresent_ShouldReturnWeightedSum()
    {
        // Arrange
        var rubricator = BuildRubricator(out var c1, out var c2);
        var studentId = StudentId.New();
        var evaluations = new List<Evaluation>
        {
            Evaluation.Create(studentId, c1, new Score(8m), DateTimeOffset.UtcNow),
            Evaluation.Create(studentId, c2, new Score(9m), DateTimeOffset.UtcNow),
        };
        var sut = new WeightedSumStrategy();

        // Act
        var result = sut.Calculate(evaluations, rubricator);

        // Assert: 8/10*0.4 + 9/10*0.6 = 0.86
        result.Value.Should().Be(0.86m);
    }

    [Fact]
    public void Calculate_WithMissingEvaluation_ShouldTreatAsZero()
    {
        // Arrange
        var rubricator = BuildRubricator(out var c1, out _);
        var evaluations = new List<Evaluation>
        {
            Evaluation.Create(StudentId.New(), c1, new Score(10m), DateTimeOffset.UtcNow),
        };
        var sut = new WeightedSumStrategy();

        // Act
        var result = sut.Calculate(evaluations, rubricator);

        // Assert: 10/10*0.4 = 0.4
        result.Value.Should().Be(0.4m);
    }

    [Fact]
    public void Calculate_WithNoEvaluations_ShouldReturnZero()
    {
        // Arrange
        var rubricator = BuildRubricator(out _, out _);
        var sut = new WeightedSumStrategy();

        // Act
        var result = sut.Calculate(Array.Empty<Evaluation>(), rubricator);

        // Assert
        result.Value.Should().Be(0m);
    }

    [Fact]
    public void Calculate_WithEmptyRubricator_ShouldReturnZero()
    {
        // Arrange
        var rubricator = new Rubricator();
        var sut = new WeightedSumStrategy();

        // Act
        var result = sut.Calculate(Array.Empty<Evaluation>(), rubricator);

        // Assert
        result.Value.Should().Be(0m);
    }

    [Fact]
    public void Name_Get_ShouldReturnWeightedSum()
    {
        // Arrange
        var sut = new WeightedSumStrategy();

        // Act
        var name = sut.Name;

        // Assert
        name.Should().Be("WeightedSum");
    }
}