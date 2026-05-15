using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Repositories;

public class InMemoryApplianceRepository : IApplianceRepository
{
    private readonly List<Appliance> _store = [];
    private int _nextId = 1;

    public Task<IEnumerable<Appliance>> GetAllAsync() =>
        Task.FromResult<IEnumerable<Appliance>>(_store.ToList());

    public Task<Appliance?> GetByIdAsync(int id) =>
        Task.FromResult(_store.FirstOrDefault(a => a.Id == id));

    public Task<bool> ExistsAsync(string name) =>
        Task.FromResult(_store.Any(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));

    public Task AddAsync(Appliance appliance)
    {
        appliance.Id = _nextId++;
        appliance.CreatedAt = DateTime.UtcNow;
        appliance.UpdatedAt = DateTime.UtcNow;
        _store.Add(appliance);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Appliance appliance)
    {
        var index = _store.FindIndex(a => a.Id == appliance.Id);
        if (index < 0) return Task.CompletedTask;
        appliance.UpdatedAt = DateTime.UtcNow;
        _store[index] = appliance;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        _store.RemoveAll(a => a.Id == id);
        return Task.CompletedTask;
    }
}
