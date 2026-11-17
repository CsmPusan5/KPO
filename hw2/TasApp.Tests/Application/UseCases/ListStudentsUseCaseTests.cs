using FluentAssertions;
using NSubstitute;
using TasApp.Application.UseCases;
using TasApp.Domain.Entities;
using TasApp.Domain.Ports;
using TasApp.Domain.ValueObjects;

namespace TasApp.Tests.Application.UseCases;

public sealed class ListStudentsUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithStudents_ShouldReturnMappedDtos()
    {
        // Arrange
        var students = Substitute.For<IStudentRepository>();
        var student1 = Student.Create(new StudentName("Ivan"));
        var student2 = Student.Create(new StudentName("Petr"));
        students.ListAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Student> { student1, student2 });

        var sut = new ListStudentsUseCase(students);

        // Act
        var result = await sut.ExecuteAsync();

        // Assert
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Ivan");
        result[1].Name.Should().Be("Petr");
        result[0].Id.Should().Be(student1.Id.Value);
    }

    [Fact]
    public async Task ExecuteAsync_WithNoStudents_ShouldReturnEmptyList()
    {
        // Arrange
        var students = Substitute.For<IStudentRepository>();
        students.ListAsync(Arg.Any<CancellationToken>()).Returns(new List<Student>());
        var sut = new ListStudentsUseCase(students);

        // Act
        var result = await sut.ExecuteAsync();

        // Assert
        result.Should().BeEmpty();
    }
}