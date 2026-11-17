using TasApp.Domain.Entities;
using TasApp.Domain.Ports;
using TasApp.Domain.ValueObjects;
using TasApp.Infrastructure.Persistence.Dto;

namespace TasApp.Infrastructure.Persistence;

public sealed class JsonStudentRepository : IStudentRepository
{
    private readonly JsonFileStore<List<StudentRecord>> _store;

    public JsonStudentRepository(string filePath)
        => _store = new JsonFileStore<List<StudentRecord>>(filePath);

    public async Task AddAsync(Student student, CancellationToken ct = default)
    {
        var records = await _store.LoadAsync(ct) ?? new List<StudentRecord>();
        records.Add(ToRecord(student));
        await _store.SaveAsync(records, ct);
    }

    public async Task<Student?> GetByIdAsync(StudentId id, CancellationToken ct = default)
    {
        var records = await _store.LoadAsync(ct) ?? new List<StudentRecord>();
        var record = records.FirstOrDefault(r => r.Id == id.Value);
        return record is null ? null : ToDomain(record);
    }

    public async Task<IReadOnlyList<Student>> ListAsync(CancellationToken ct = default)
    {
        var records = await _store.LoadAsync(ct) ?? new List<StudentRecord>();
        return records.Select(ToDomain).ToList();
    }

    public async Task UpdateAsync(Student student, CancellationToken ct = default)
    {
        var records = await _store.LoadAsync(ct) ?? new List<StudentRecord>();
        var index = records.FindIndex(r => r.Id == student.Id.Value);
        if (index < 0)
            throw new InvalidOperationException($"Student '{student.Id.Value}' not found");

        records[index] = ToRecord(student);
        await _store.SaveAsync(records, ct);
    }

    private static StudentRecord ToRecord(Student s)
        => new(s.Id.Value, s.Name.Value);

    private static Student ToDomain(StudentRecord r)
        => Student.Rehydrate(new StudentId(r.Id), new StudentName(r.Name));
}