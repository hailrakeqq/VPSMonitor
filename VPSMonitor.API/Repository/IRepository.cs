namespace VPSMonitor.API.Repository;

public interface IRepository<T> : IDisposable where T : class
{
    List<T> GetList();
    Task<T> GetItemById(string id);
    Task Create(T item);
    Task Update(T item);
    Task Delete(string id);
    void Save();
}