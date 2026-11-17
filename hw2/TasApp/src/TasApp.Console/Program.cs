using Microsoft.Extensions.DependencyInjection;
using TasApp.Application.Facades;
using TasApp.Application.Ports;
using TasApp.Application.UseCases;
using TasApp.Console;
using TasApp.Domain.Ports;
using TasApp.Domain.Services;
using TasApp.Infrastructure.Export;
using TasApp.Infrastructure.Persistence;

var services = new ServiceCollection();

// --- Domain services ---
services.AddSingleton<IFinalScoreStrategy, WeightedSumStrategy>();
services.AddSingleton<FinalScoreCalculator>();

// --- Repositories (singleton: одно файловое хранилище на всё приложение) ---
services.AddSingleton<IStudentRepository>(_ =>
    new JsonStudentRepository("data/students.json"));
services.AddSingleton<IRubricatorRepository>(_ =>
    new JsonRubricatorRepository("data/rubricator.json"));
services.AddSingleton<IEvaluationRepository>(_ =>
    new JsonEvaluationRepository("data/evaluations.json"));

// --- Exporter ---
services.AddSingleton<IFinalScoreExporter, CsvFinalScoreExporter>();

// --- Use cases (transient: без состояния) ---
services.AddTransient<AddStudentUseCase>();
services.AddTransient<AddCriterionUseCase>();
services.AddTransient<ListStudentsUseCase>();
services.AddTransient<ListCriteriaUseCase>();
services.AddTransient<AssignEvaluationUseCase>();
services.AddTransient<UpdateEvaluationUseCase>();
services.AddTransient<DeleteEvaluationUseCase>();
services.AddTransient<ListEvaluationsUseCase>();
services.AddTransient<CalculateFinalScoreUseCase>();
services.AddTransient<ExportFinalScoreUseCase>();

// --- Facade ---
services.AddTransient<TasFacade>();

// --- Console ---
services.AddSingleton<ConsoleController>();

var provider = services.BuildServiceProvider();
var controller = provider.GetRequiredService<ConsoleController>();

await controller.RunAsync(args);