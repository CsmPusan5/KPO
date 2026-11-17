using TasApp.Domain.ValueObjects;

namespace TasApp.Domain.Entities;

public sealed class Student
{
    public StudentId Id { get; }
    public StudentName Name { get; private set; }

    private Student(StudentId id, StudentName name)
    {
        Id = id;
        Name = name;
    }

    // Создание нового студента (генерирует новый Id).
    public static Student Create(StudentName name)
        => new(StudentId.New(), name);

    // Восстановление из хранилища (Id сохраняется).
    public static Student Rehydrate(StudentId id, StudentName name)
        => new(id, name);

    public void Rename(StudentName newName) => Name = newName;

    public override string ToString() => $"Student({Id.Value:N}): {Name.Value}";
}