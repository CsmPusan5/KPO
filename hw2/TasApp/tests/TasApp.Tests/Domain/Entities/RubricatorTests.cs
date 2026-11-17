using FluentAssertions;
using TasApp.Domain.Entities;
using TasApp.Domain.Exceptions;
using TasApp.Domain.ValueObjects;

namespace TasApp.Tests.Domain.Entities;

public sealed class RubricatorTests
{
    [Fact]
    public void Ctor_Call_ShouldCreateEmptyRubricator()
    {
        // Arrange

        // Act
        var rubricator = new Rubricator();

        // Assert
        rubricator.Criteria.Should().BeEmpty();
    }

    [Fact]
    public void AddCriterion_WithValidData_ShouldReturnCriterionAndAddToList()
    {
        // Arrange
        var rubricator = new Rubricator();

        // Act
        var criterion = rubricator.AddCriterion(
            new CriterionName("ДЗ"),
            new MaxScore(10m),
            new Weight(0.4m));

        // Assert
        rubricator.Criteria.Should().HaveCount(1);
        rubricator.Criteria[0].Should().BeSameAs(criterion);
    }

    [Fact]
    public void AddCriterion_CalledTwice_ShouldGenerateUniqueIds()
    {
        // Arrange
        var rubricator = new Rubricator();

        // Act
        var a = rubricator.AddCriterion(new CriterionName("A"), new MaxScore(10m), new Weight(0.5m));
        var b = rubricator.AddCriterion(new CriterionName("B"), new MaxScore(10m), new Weight(0.5m));

        // Assert
        a.Id.Should().NotBe(b.Id);
    }

    [Fact]
    public void GetCriterion_WithExistingId_ShouldReturnCriterion()
    {
        // Arrange
        var rubricator = new Rubricator();
        var criterion = rubricator.AddCriterion(
            new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m));

        // Act
        var result = rubricator.GetCriterion(criterion.Id);

        // Assert
        result.Should().BeSameAs(criterion);
    }

    [Fact]
    public void GetCriterion_WithMissingId_ShouldThrowCriterionNotFound()
    {
        // Arrange
        var rubricator = new Rubricator();
        var missingId = CriterionId.New();

        // Act
        Action act = () => rubricator.GetCriterion(missingId);

        // Assert
        act.Should().Throw<CriterionNotFoundInRubricatorException>();
    }

    [Fact]
    public void ContainsCriterion_WithMissingId_ShouldReturnFalse()
    {
        // Arrange
        var rubricator = new Rubricator();
        var missingId = CriterionId.New();

        // Act
        var contains = rubricator.ContainsCriterion(missingId);

        // Assert
        contains.Should().BeFalse();
    }

    [Fact]
    public void UpdateCriterion_WithNewData_ShouldChangeAllFields()
    {
        // Arrange
        var rubricator = new Rubricator();
        var criterion = rubricator.AddCriterion(
            new CriterionName("Old"), new MaxScore(10m), new Weight(0.4m));

        // Act
        rubricator.UpdateCriterion(
            criterion.Id,
            new CriterionName("New"),
            new MaxScore(20m),
            new Weight(0.6m));

        // Assert
        var updated = rubricator.GetCriterion(criterion.Id);
        updated.Name.Value.Should().Be("New");
        updated.MaxScore.Value.Should().Be(20m);
        updated.Weight.Value.Should().Be(0.6m);
    }

    [Fact]
    public void RemoveCriterion_WithExistingId_ShouldRemoveFromList()
    {
        // Arrange
        var rubricator = new Rubricator();
        var criterion = rubricator.AddCriterion(
            new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m));

        // Act
        rubricator.RemoveCriterion(criterion.Id);

        // Assert
        rubricator.Criteria.Should().BeEmpty();
    }

    [Fact]
    public void RemoveCriterion_WithMissingId_ShouldThrowCriterionNotFound()
    {
        // Arrange
        var rubricator = new Rubricator();
        var missingId = CriterionId.New();

        // Act
        Action act = () => rubricator.RemoveCriterion(missingId);

        // Assert
        act.Should().Throw<CriterionNotFoundInRubricatorException>();
    }
}