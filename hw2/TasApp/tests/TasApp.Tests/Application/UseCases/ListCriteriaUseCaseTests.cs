using FluentAssertions;
using NSubstitute;
using TasApp.Application.UseCases;
using TasApp.Domain.Entities;
using TasApp.Domain.Ports;
using TasApp.Domain.ValueObjects;

namespace TasApp.Tests.Application.UseCases;

public sealed class ListCriteriaUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithCriteria_ShouldReturnMappedDtos()
    {
        // Arrange
        var rubricator = new Rubricator();
        rubricator.AddCriterion(new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m));
        rubricator.AddCriterion(new CriterionName("Экзамен"), new MaxScore(20m), new Weight(0.6m));

        var rubricators = Substitute.For<IRubricatorRepository>();
        rubricators.GetAsync(Arg.Any<CancellationToken>()).Returns(rubricator);

        var sut = new ListCriteriaUseCase(rubricators);

        // Act
        var result = await sut.ExecuteAsync();

        // Assert
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("ДЗ");
        result[0].MaxScore.Should().Be(10m);
        result[1].Name.Should().Be("Экзамен");
        result[1].Weight.Should().Be(0.6m);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyRubricator_ShouldReturnEmptyList()
    {
        // Arrange
        var rubricators = Substitute.For<IRubricatorRepository>();
        rubricators.GetAsync(Arg.Any<CancellationToken>()).Returns(new Rubricator());
        var sut = new ListCriteriaUseCase(rubricators);

        // Act
        var result = await sut.ExecuteAsync();

        // Assert
        result.Should().BeEmpty();
    }
}