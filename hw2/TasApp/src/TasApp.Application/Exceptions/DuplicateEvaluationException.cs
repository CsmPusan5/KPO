namespace TasApp.Application.Exceptions;

public sealed class DuplicateEvaluationException : TasApplicationException
{
    public DuplicateEvaluationException(Guid studentId, Guid criterionId)
        : base($"Evaluation for student '{studentId}' and criterion '{criterionId}' already exists") { }
}