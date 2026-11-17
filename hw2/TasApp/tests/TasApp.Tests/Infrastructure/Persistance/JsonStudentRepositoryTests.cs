using FluentAssertions;
using TasApp.Domain.Entities;
using TasApp.Domain.ValueObjects;
using TasApp.Infrastructure.Persistence;

namespace TasApp.Tests.Infrastructure.Persistence;

public sealed class JsonStudentRepositoryTests : IDisposable
{
    private readonly string _filePath;
    private readonly JsonStudentRepository _sut;

    public JsonStudentRepositoryTests()
    {
        _filePath = Path.Combine(Path.GetTempPath(), $"students-{Guid.NewGuid():N}.json");
        _sut = new JsonStudentRepository(_filePath);
    }

    public void Dispose()
    {
        if (File.Exists(_filePath)) File.Delete(_filePath);
    }

    [Fact]
    public async Task AddAsync_WithStudent_ShouldPersistToFile()
    {
        // Arrange
        var student = Student.Create(new StudentName("Ivan"));

        // Act
        await _sut.AddAsync(student);

        // Assert
        File.Exists(_filePath).Should().BeTrue();
        var loaded = await _sut.GetByIdAsync(student.Id);
        loaded.Should().NotBeNull();
        loaded!.Name.Value.Should().Be("Ivan");
    }

    [Fact]
    public async Task GetByIdAsync_WithMissingId_ShouldReturnNull()
    {
        // Arrange
        var missingId = StudentId.New();

        // Act
        var result = await _sut.GetByIdAsync(missingId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ListAsync_WithNoFile_ShouldReturnEmpty()
    {
        // Arrange

        // Act
        var result = await _sut.ListAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ListAsync_WithMultipleStudents_ShouldReturnAll()
    {
        // Arrange
        await _sut.AddAsync(Student.Create(new StudentName("Ivan")));
        await _sut.AddAsync(Student.Create(new StudentName("Petr")));

        // Act
        var result = await _sut.ListAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Select(s => s.Name.Value).Should().BeEquivalentTo(new[] { "Ivan", "Petr" });
    }

    [Fact]
    public async Task UpdateAsync_WithExistingStudent_ShouldPersistChanges()
    {
        // Arrange
        var student = Student.Create(new StudentName("Ivan"));
        await _sut.AddAsync(student);
        student.Rename(new StudentName("Ivan Petrov"));

        // Act
        await _sut.UpdateAsync(student);

        // Assert
        var loaded = await _sut.GetByIdAsync(student.Id);
        loaded!.Name.Value.Should().Be("Ivan Petrov");
    }

    [Fact]
    public async Task UpdateAsync_WithMissingStudent_ShouldThrowInvalidOperation()
    {
        // Arrange
        var student = Student.Create(new StudentName("Ivan"));

        // Act
        Func<Task> act = async () => await _sut.UpdateAsync(student);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}