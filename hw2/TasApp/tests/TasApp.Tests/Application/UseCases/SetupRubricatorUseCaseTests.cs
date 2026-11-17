using FluentAssertions;
using NSubstitute;
using TasApp.Application.Commands;
using TasApp.Application.UseCases;
using TasApp.Domain.Entities;
using TasApp.Domain.Exceptions;
using TasApp.Domain.Ports;

namespace TasApp.Tests.Application.UseCases;

public sealed class SetupRubricatorUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidCriteria_ShouldSaveRubricatorAndReturnDtos()
    {
        // Arrange
        var rubricators = Substitute.For<IRubricatorRepository>();
        var sut = new SetupRubricatorUseCase(rubricators);
        var cmd = new SetupRubricatorCommand(new[]
        {
            new SetupCriterionCommand("ДЗ", 10m, 0.4m),
            new SetupCriterionCommand("Экзамен", 20m, 0.6m),
        });

        // Act
        var result = await sut.ExecuteAsync(cmd);

        // Assert
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("ДЗ");
        result[0].MaxScore.Should().Be(10m);
        result[1].Name.Should().Be("Экзамен");
        await rubricators.Received(1).SaveAsync(
            Arg.Is<Rubricator>(r => r.Criteria.Count == 2),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithDuplicates_ShouldThrowAndNotSave()
    {
        // Arrange
        var rubricators = Substitute.For<IRubricatorRepository>();
        var sut = new SetupRubricatorUseCase(rubricators);
        var cmd = new SetupRubricatorCommand(new[]
        {
            new SetupCriterionCommand("ДЗ", 10m, 0.4m),
            new SetupCriterionCommand("ДЗ", 20m, 0.6m),
        });

        // Act
        Func<Task> act = async () => await sut.ExecuteAsync(cmd);

        // Assert
        await act.Should().ThrowAsync<DuplicateCriterionNameException>();
        await rubricators.DidNotReceive()
            .SaveAsync(Arg.Any<Rubricator>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyList_ShouldSaveEmptyRubricator()
    {
        // Arrange
        var rubricators = Substitute.For<IRubricatorRepository>();
        var sut = new SetupRubricatorUseCase(rubricators);
        var cmd = new SetupRubricatorCommand(Array.Empty<SetupCriterionCommand>());

        // Act
        var result = await sut.ExecuteAsync(cmd);

        // Assert
        result.Should().BeEmpty();
        await rubricators.Received(1).SaveAsync(
            Arg.Is<Rubricator>(r => r.Criteria.Count == 0),
            Arg.Any<CancellationToken>());
    }
}