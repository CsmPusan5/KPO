using FluentAssertions;
using NSubstitute;
using TasApp.Application.Commands;
using TasApp.Application.UseCases;
using TasApp.Domain.Entities;
using TasApp.Domain.Exceptions;
using TasApp.Domain.Ports;

namespace TasApp.Tests.Application.UseCases;

public sealed class AddStudentUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidName_ShouldSaveStudent()
    {
        // Arrange
        var students = Substitute.For<IStudentRepository>();
        var sut = new AddStudentUseCase(students);
        var cmd = new AddStudentCommand("Ivan");

        // Act
        var result = await sut.ExecuteAsync(cmd);

        // Assert
        result.Value.Should().NotBe(Guid.Empty);
        await students.Received(1).AddAsync(
            Arg.Is<Student>(s => s.Name.Value == "Ivan"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyName_ShouldThrowInvalidStudentName()
    {
        // Arrange
        var students = Substitute.For<IStudentRepository>();
        var sut = new AddStudentUseCase(students);
        var cmd = new AddStudentCommand("   ");

        // Act
        Func<Task> act = async () => await sut.ExecuteAsync(cmd);

        // Assert
        await act.Should().ThrowAsync<InvalidStudentNameException>();
        await students.DidNotReceive().AddAsync(Arg.Any<Student>(), Arg.Any<CancellationToken>());
    }
}