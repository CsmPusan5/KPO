using FluentAssertions;
using TasApp.Domain.Entities;
using TasApp.Domain.Services;
using TasApp.Domain.ValueObjects;

namespace TasApp.Tests.Domain.Services;

public sealed class FinalScoreCalculatorTests
{
    [Fact]
    public void Calculate_Call_ShouldDelegateToStrategy()
    {
        // Arrange
        var rubricator = new Rubricator();
        var c1 = rubricator.AddCriterion(new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m));
        var evaluations = new List<Evaluation>
        {
            Evaluation.Create(StudentId.New(), c1, new Score(10m), DateTimeOffset.UtcNow),
        };
        var calculator = new FinalScoreCalculator(new WeightedSumStrategy());

        // Act
        var result = calculator.Calculate(evaluations, rubricator);

        // Assert
        result.Value.Should().Be(0.4m);
    }

    [Fact]
    public void StrategyName_Get_ShouldReturnStrategyName()
    {
        // Arrange
        var calculator = new FinalScoreCalculator(new WeightedSumStrategy());

        // Act
        var name = calculator.StrategyName;

        // Assert
        name.Should().Be("WeightedSum");
    }

    [Fact]
    public void Ctor_WithNullStrategy_ShouldThrowArgumentNullException()
    {
        // Arrange
        IFinalScoreStrategy? strategy = null;

        // Act
        Action act = () => new FinalScoreCalculator(strategy!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}