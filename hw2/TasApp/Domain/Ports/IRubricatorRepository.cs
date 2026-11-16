using TasApp.Domain.Entities;

namespace TasApp.Domain.Ports;

public interface IRubricatorRepository
{
    Task<Rubricator> GetAsync(CancellationToken ct = default);
    Task SaveAsync(Rubricator rubricator, CancellationToken ct = default);
}