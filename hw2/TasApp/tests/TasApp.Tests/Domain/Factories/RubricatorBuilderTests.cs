using FluentAssertions;
using TasApp.Domain.Exceptions;
using TasApp.Domain.Factories;
using TasApp.Domain.ValueObjects;

namespace TasApp.Tests.Domain.Factories;

public sealed class RubricatorBuilderTests
{
    [Fact]
    public void Build_WithNoCriteria_ShouldReturnEmptyRubricator()
    {
        // Arrange
        var builder = new RubricatorBuilder();

        // Act
        var result = builder.Build();

        // Assert
        result.Criteria.Should().BeEmpty();
    }

    [Fact]
    public void Build_WithOneCriterion_ShouldReturnRubricatorWithOne()
    {
        // Arrange
        var builder = new RubricatorBuilder()
            .AddCriterion(new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m));

        // Act
        var result = builder.Build();

        // Assert
        result.Criteria.Should().HaveCount(1);
        result.Criteria[0].Name.Value.Should().Be("ДЗ");
        result.Criteria[0].MaxScore.Value.Should().Be(10m);
        result.Criteria[0].Weight.Value.Should().Be(0.4m);
    }

    [Fact]
    public void Build_WithMultipleCriteria_ShouldReturnAll()
    {
        // Arrange
        var builder = new RubricatorBuilder()
            .AddCriterion(new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m))
            .AddCriterion(new CriterionName("Экзамен"), new MaxScore(20m), new Weight(0.6m));

        // Act
        var result = builder.Build();

        // Assert
        result.Criteria.Should().HaveCount(2);
        result.Criteria[0].Name.Value.Should().Be("ДЗ");
        result.Criteria[1].Name.Value.Should().Be("Экзамен");
    }

    [Fact]
    public void Build_WithDuplicateNames_ShouldThrowDuplicateCriterionName()
    {
        // Arrange
        var builder = new RubricatorBuilder()
            .AddCriterion(new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m))
            .AddCriterion(new CriterionName("ДЗ"), new MaxScore(20m), new Weight(0.6m));

        // Act
        Action act = () => builder.Build();

        // Assert
        act.Should().Throw<DuplicateCriterionNameException>()
            .Which.DuplicateNames.Should().ContainSingle().Which.Should().Be("ДЗ");
    }

    [Fact]
    public void Build_WithThreeDuplicates_ShouldListAllDuplicatedNames()
    {
        // Arrange
        var builder = new RubricatorBuilder()
            .AddCriterion(new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m))
            .AddCriterion(new CriterionName("Экзамен"), new MaxScore(10m), new Weight(0.3m))
            .AddCriterion(new CriterionName("ДЗ"), new MaxScore(20m), new Weight(0.2m))
            .AddCriterion(new CriterionName("Экзамен"), new MaxScore(30m), new Weight(0.1m));

        // Act
        Action act = () => builder.Build();

        // Assert
        act.Should().Throw<DuplicateCriterionNameException>()
            .Which.DuplicateNames.Should().BeEquivalentTo(new[] { "ДЗ", "Экзамен" });
    }

    [Fact]
    public void Build_CalledTwice_ShouldReturnIndependentRubricators()
    {
        // Arrange
        var builder = new RubricatorBuilder()
            .AddCriterion(new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m));

        // Act
        var first = builder.Build();
        var second = builder.Build();

        // Assert
        first.Should().NotBeSameAs(second);
        first.Criteria[0].Id.Should().NotBe(second.Criteria[0].Id);
    }

    [Fact]
    public void Build_WithNamesDifferentInCase_ShouldTreatAsDifferent()
    {
        // Arrange — сравнение по значению (case-sensitive)
        var builder = new RubricatorBuilder()
            .AddCriterion(new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m))
            .AddCriterion(new CriterionName("дз"), new MaxScore(20m), new Weight(0.6m));

        // Act
        var result = builder.Build();

        // Assert
        result.Criteria.Should().HaveCount(2);
    }
}