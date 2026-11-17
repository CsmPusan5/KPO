using FluentAssertions;
using NSubstitute;
using TasApp.Application.Commands;
using TasApp.Application.UseCases;
using TasApp.Domain.Entities;
using TasApp.Domain.Exceptions;
using TasApp.Domain.Ports;

namespace TasApp.Tests.Application.UseCases;

public sealed class AddCriterionUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldAddCriterionAndSaveRubricator()
    {
        // Arrange
        var rubricators = Substitute.For<IRubricatorRepository>();
        rubricators.GetAsync(Arg.Any<CancellationToken>()).Returns(new Rubricator());

        var sut = new AddCriterionUseCase(rubricators);
        var cmd = new AddCriterionCommand("ДЗ", 10m, 0.4m);

        // Act
        var result = await sut.ExecuteAsync(cmd);

        // Assert
        result.Value.Should().NotBe(Guid.Empty);
        await rubricators.Received(1).SaveAsync(
            Arg.Is<Rubricator>(r =>
                r.Criteria.Count == 1 &&
                r.Criteria[0].Name.Value == "ДЗ" &&
                r.Criteria[0].MaxScore.Value == 10m),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithNonPositiveMaxScore_ShouldThrowInvalidMaxScore()
    {
        // Arrange
        var rubricators = Substitute.For<IRubricatorRepository>();
        rubricators.GetAsync(Arg.Any<CancellationToken>()).Returns(new Rubricator());

        var sut = new AddCriterionUseCase(rubricators);
        var cmd = new AddCriterionCommand("ДЗ", 0m, 0.4m);

        // Act
        Func<Task> act = async () => await sut.ExecuteAsync(cmd);

        // Assert
        await act.Should().ThrowAsync<InvalidMaxScoreException>();
        await rubricators.DidNotReceive().SaveAsync(Arg.Any<Rubricator>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithNonPositiveWeight_ShouldThrowInvalidWeight()
    {
        // Arrange
        var rubricators = Substitute.For<IRubricatorRepository>();
        rubricators.GetAsync(Arg.Any<CancellationToken>()).Returns(new Rubricator());

        var sut = new AddCriterionUseCase(rubricators);
        var cmd = new AddCriterionCommand("ДЗ", 10m, 0m);

        // Act
        Func<Task> act = async () => await sut.ExecuteAsync(cmd);

        // Assert
        await act.Should().ThrowAsync<InvalidWeightException>();
    }
}