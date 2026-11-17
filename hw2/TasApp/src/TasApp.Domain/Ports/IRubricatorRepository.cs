using TasApp.Domain.Entities;

namespace TasApp.Domain.Ports;

// Репозиторий агрегата: возвращает Rubricator вместе с Criteria.
public interface IRubricatorRepository
{
    Task<Rubricator> GetAsync(CancellationToken ct = default);
    Task SaveAsync(Rubricator rubricator, CancellationToken ct = default);
}