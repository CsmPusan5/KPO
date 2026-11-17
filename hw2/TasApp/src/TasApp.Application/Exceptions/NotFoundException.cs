namespace TasApp.Application.Exceptions;

public sealed class StudentNotFoundException : TasApplicationException
{
    public StudentNotFoundException(Guid id)
        : base($"Student '{id}' not found") { }
}

public sealed class EvaluationNotFoundException : TasApplicationException
{
    public EvaluationNotFoundException(Guid id)
        : base($"Evaluation '{id}' not found") { }
}