using FluentAssertions;
using TasApp.Domain.Entities;
using TasApp.Domain.ValueObjects;

namespace TasApp.Tests.Domain.Entities;

public sealed class StudentTests
{
    [Fact]
    public void Create_Call_ShouldGenerateNewId()
    {
        // Arrange
        var name = new StudentName("Ivan");

        // Act
        var student = Student.Create(name);

        // Assert
        student.Id.Value.Should().NotBe(Guid.Empty);
        student.Name.Value.Should().Be("Ivan");
    }

    [Fact]
    public void Create_CalledTwice_ShouldGenerateDifferentIds()
    {
        // Arrange

        // Act
        var a = Student.Create(new StudentName("A"));
        var b = Student.Create(new StudentName("B"));

        // Assert
        a.Id.Should().NotBe(b.Id);
    }

    [Fact]
    public void Rename_WithNewName_ShouldChangeName()
    {
        // Arrange
        var student = Student.Create(new StudentName("Old"));

        // Act
        student.Rename(new StudentName("New"));

        // Assert
        student.Name.Value.Should().Be("New");
    }

    [Fact]
    public void Rehydrate_Call_ShouldPreserveId()
    {
        // Arrange
        var id = StudentId.New();
        var name = new StudentName("Ivan");

        // Act
        var student = Student.Rehydrate(id, name);

        // Assert
        student.Id.Should().Be(id);
        student.Name.Value.Should().Be("Ivan");
    }
}