using FluentAssertions;
using TasApp.Domain.Entities;
using TasApp.Domain.ValueObjects;
using TasApp.Infrastructure.Persistence;

namespace TasApp.Tests.Infrastructure.Persistence;

public sealed class JsonRubricatorRepositoryTests : IDisposable
{
    private readonly string _filePath;
    private readonly JsonRubricatorRepository _sut;

    public JsonRubricatorRepositoryTests()
    {
        _filePath = Path.Combine(Path.GetTempPath(), $"rubricator-{Guid.NewGuid():N}.json");
        _sut = new JsonRubricatorRepository(_filePath);
    }

    public void Dispose()
    {
        if (File.Exists(_filePath)) File.Delete(_filePath);
    }

    [Fact]
    public async Task GetAsync_WithNoFile_ShouldReturnEmptyRubricator()
    {
        // Arrange

        // Act
        var result = await _sut.GetAsync();

        // Assert
        result.Should().NotBeNull();
        result.Criteria.Should().BeEmpty();
    }

    [Fact]
    public async Task SaveAsync_WithCriteria_ShouldPersistToFile()
    {
        // Arrange
        var rubricator = new Rubricator();
        rubricator.AddCriterion(new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m));
        rubricator.AddCriterion(new CriterionName("Экзамен"), new MaxScore(20m), new Weight(0.6m));

        // Act
        await _sut.SaveAsync(rubricator);

        // Assert
        File.Exists(_filePath).Should().BeTrue();

        var loaded = await _sut.GetAsync();
        loaded.Criteria.Should().HaveCount(2);
        loaded.Criteria[0].Name.Value.Should().Be("ДЗ");
        loaded.Criteria[0].MaxScore.Value.Should().Be(10m);
        loaded.Criteria[0].Weight.Value.Should().Be(0.4m);
        loaded.Criteria[1].Name.Value.Should().Be("Экзамен");
    }

    [Fact]
    public async Task SaveAsync_CalledTwice_ShouldOverwriteNotAppend()
    {
        // Arrange
        var first = new Rubricator();
        first.AddCriterion(new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m));
        await _sut.SaveAsync(first);

        var second = new Rubricator();
        second.AddCriterion(new CriterionName("Экзамен"), new MaxScore(20m), new Weight(0.6m));

        // Act
        await _sut.SaveAsync(second);

        // Assert
        var loaded = await _sut.GetAsync();
        loaded.Criteria.Should().HaveCount(1);
        loaded.Criteria[0].Name.Value.Should().Be("Экзамен");
    }

    [Fact]
    public async Task SaveAsync_ThenGet_ShouldPreserveCriterionIds()
    {
        // Arrange
        var rubricator = new Rubricator();
        var original = rubricator.AddCriterion(
            new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m));
        await _sut.SaveAsync(rubricator);

        // Act
        var loaded = await _sut.GetAsync();

        // Assert
        loaded.Criteria[0].Id.Should().Be(original.Id);
    }
}