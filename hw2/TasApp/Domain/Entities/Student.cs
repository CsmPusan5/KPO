using TasApp.Domain.ValueObjects;

namespace TasApp.Domain.Entities;

/// <summary>
/// Студент — кого оценивают.
/// Entity с идентичностью; одновременно — корень своего агрегата.
/// </summary>
public sealed class Student
{
    public StudentId Id { get; }
    public StudentName Name { get; private set; }

    private Student(StudentId id, StudentName name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>Создание нового студента (генерирует новый Id).</summary>
    public static Student Create(StudentName name)
        => new(StudentId.New(), name);

    /// <summary>Восстановление из хранилища (Id сохраняется).</summary>
    public static Student Rehydrate(StudentId id, StudentName name)
        => new(id, name);

    /// <summary>Переименование. Имя валидируется в VO.</summary>
    public void Rename(StudentName newName) => Name = newName;

    public override string ToString() => $"Student({Id.Value:N}): {Name.Value}";
}