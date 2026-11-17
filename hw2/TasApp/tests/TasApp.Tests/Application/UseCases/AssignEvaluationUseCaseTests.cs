using FluentAssertions;
using NSubstitute;
using TasApp.Application.Commands;
using TasApp.Application.Exceptions;
using TasApp.Application.UseCases;
using TasApp.Domain.Entities;
using TasApp.Domain.Exceptions;
using TasApp.Domain.Ports;
using TasApp.Domain.ValueObjects;

namespace TasApp.Tests.Application.UseCases;

public sealed class AssignEvaluationUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldSaveEvaluation()
    {
        // Arrange
        var student = Student.Create(new StudentName("Ivan"));
        var rubricator = new Rubricator();
        var criterion = rubricator.AddCriterion(
            new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m));

        var students = Substitute.For<IStudentRepository>();
        var rubricators = Substitute.For<IRubricatorRepository>();
        var evaluations = Substitute.For<IEvaluationRepository>();
        students.GetByIdAsync(Arg.Any<StudentId>(), Arg.Any<CancellationToken>()).Returns(student);
        rubricators.GetAsync(Arg.Any<CancellationToken>()).Returns(rubricator);

        var sut = new AssignEvaluationUseCase(students, rubricators, evaluations);
        var cmd = new AssignEvaluationCommand(student.Id.Value, criterion.Id.Value, 5m, "ok");

        // Act
        var result = await sut.ExecuteAsync(cmd);

        // Assert
        result.Value.Should().NotBe(Guid.Empty);
        await evaluations.Received(1).AddAsync(
            Arg.Is<Evaluation>(e =>
                e.StudentId == student.Id &&
                e.CriterionId == criterion.Id &&
                e.Score.Value == 5m &&
                e.Comment == "ok"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithMissingStudent_ShouldThrowStudentNotFound()
    {
        // Arrange
        var students = Substitute.For<IStudentRepository>();
        var rubricators = Substitute.For<IRubricatorRepository>();
        var evaluations = Substitute.For<IEvaluationRepository>();
        students.GetByIdAsync(Arg.Any<StudentId>(), Arg.Any<CancellationToken>())
            .Returns((Student?)null);

        var sut = new AssignEvaluationUseCase(students, rubricators, evaluations);
        var cmd = new AssignEvaluationCommand(Guid.NewGuid(), Guid.NewGuid(), 5m, null);

        // Act
        Func<Task> act = async () => await sut.ExecuteAsync(cmd);

        // Assert
        await act.Should().ThrowAsync<StudentNotFoundException>();
        await evaluations.DidNotReceive().AddAsync(Arg.Any<Evaluation>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithMissingCriterion_ShouldThrowCriterionNotFound()
    {
        // Arrange
        var student = Student.Create(new StudentName("Ivan"));
        var rubricator = new Rubricator();

        var students = Substitute.For<IStudentRepository>();
        var rubricators = Substitute.For<IRubricatorRepository>();
        var evaluations = Substitute.For<IEvaluationRepository>();
        students.GetByIdAsync(Arg.Any<StudentId>(), Arg.Any<CancellationToken>()).Returns(student);
        rubricators.GetAsync(Arg.Any<CancellationToken>()).Returns(rubricator);

        var sut = new AssignEvaluationUseCase(students, rubricators, evaluations);
        var cmd = new AssignEvaluationCommand(student.Id.Value, Guid.NewGuid(), 5m, null);

        // Act
        Func<Task> act = async () => await sut.ExecuteAsync(cmd);

        // Assert
        await act.Should().ThrowAsync<CriterionNotFoundInRubricatorException>();
        await evaluations.DidNotReceive().AddAsync(Arg.Any<Evaluation>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithScoreExceedingMax_ShouldThrowScoreOutOfRange()
    {
        // Arrange
        var student = Student.Create(new StudentName("Ivan"));
        var rubricator = new Rubricator();
        var criterion = rubricator.AddCriterion(
            new CriterionName("ДЗ"), new MaxScore(10m), new Weight(0.4m));

        var students = Substitute.For<IStudentRepository>();
        var rubricators = Substitute.For<IRubricatorRepository>();
        var evaluations = Substitute.For<IEvaluationRepository>();
        students.GetByIdAsync(Arg.Any<StudentId>(), Arg.Any<CancellationToken>()).Returns(student);
        rubricators.GetAsync(Arg.Any<CancellationToken>()).Returns(rubricator);

        var sut = new AssignEvaluationUseCase(students, rubricators, evaluations);
        var cmd = new AssignEvaluationCommand(student.Id.Value, criterion.Id.Value, 12m, null);

        // Act
        Func<Task> act = async () => await sut.ExecuteAsync(cmd);

        // Assert
        await act.Should().ThrowAsync<ScoreOutOfRangeException>();
        await evaluations.DidNotReceive().AddAsync(Arg.Any<Evaluation>(), Arg.Any<CancellationToken>());
    }
}