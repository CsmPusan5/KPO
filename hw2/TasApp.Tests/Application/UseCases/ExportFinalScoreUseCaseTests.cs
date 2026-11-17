using FluentAssertions;
using NSubstitute;
using TasApp.Application.Commands;
using TasApp.Application.Dtos;
using TasApp.Application.Ports;
using TasApp.Application.UseCases;
using TasApp.Domain.Entities;
using TasApp.Domain.Exceptions;
using TasApp.Domain.Ports;
using TasApp.Domain.Services;
using TasApp.Domain.ValueObjects;

namespace TasApp.Tests.Application.UseCases;

public sealed class ExportFinalScoreUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldCalculateAndExport()
    {
        // Arrange
        var rubricator = new Rubricator();
        var c1 = rubricator.AddCriterion(new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m));
        var studentId = StudentId.New();
        var evaluations = new List<Evaluation>
        {
            Evaluation.Create(studentId, c1, new Score(10m), DateTimeOffset.UtcNow),
        };

        var evalRepo = Substitute.For<IEvaluationRepository>();
        var rubRepo = Substitute.For<IRubricatorRepository>();
        evalRepo.ListByStudentAsync(Arg.Any<StudentId>(), Arg.Any<CancellationToken>())
            .Returns(evaluations);
        rubRepo.GetAsync(Arg.Any<CancellationToken>()).Returns(rubricator);

        var calculator = new FinalScoreCalculator(new WeightedSumStrategy());
        var calculate = new CalculateFinalScoreUseCase(evalRepo, rubRepo, calculator);

        var exporter = Substitute.For<IFinalScoreExporter>();
        var sut = new ExportFinalScoreUseCase(calculate, exporter);
        var cmd = new ExportFinalScoreCommand(studentId.Value, "out/report.csv");

        // Act
        await sut.ExecuteAsync(cmd);

        // Assert
        await exporter.Received(1).ExportAsync(
            Arg.Is<FinalScoreDto>(d => d.StudentId == studentId.Value && d.Value == 0.4m),
            "out/report.csv",
            Arg.Any<CancellationToken>());
    }

        [Fact]
    public async Task ExecuteAsync_WhenCalculateThrowsMissingEvaluations_ShouldNotExport()
    {
        // Arrange
        var rubricator = new Rubricator();
        rubricator.AddCriterion(new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m));

        var evalRepo = Substitute.For<IEvaluationRepository>();
        var rubRepo = Substitute.For<IRubricatorRepository>();
        evalRepo.ListByStudentAsync(Arg.Any<StudentId>(), Arg.Any<CancellationToken>())
            .Returns(new List<Evaluation>());
        rubRepo.GetAsync(Arg.Any<CancellationToken>()).Returns(rubricator);

        var calculator = new FinalScoreCalculator(new StrictWeightedSumStrategy());
        var calculate = new CalculateFinalScoreUseCase(evalRepo, rubRepo, calculator);
        var exporter = Substitute.For<IFinalScoreExporter>();
        var sut = new ExportFinalScoreUseCase(calculate, exporter);
        var cmd = new ExportFinalScoreCommand(Guid.NewGuid(), "out/report.csv");

        // Act
        Func<Task> act = async () => await sut.ExecuteAsync(cmd);

        // Assert
        await act.Should().ThrowAsync<MissingEvaluationsException>();
        await exporter.DidNotReceive().ExportAsync(
            Arg.Any<FinalScoreDto>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }
}