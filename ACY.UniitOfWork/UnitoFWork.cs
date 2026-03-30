namespace ACY.UnitOfWork;

// Repository interface
public interface IRepository<T> where T : class
{
    Task AddAsync(T entity);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(object id);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}

// Unit of Work interface
public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : class;
    Task<int> CommitAsync();
    void Rollback();
}

// In-memory repository implementasyonu (örnek)
public class InMemoryRepository<T> : IRepository<T> where T : class
{
    private readonly List<T> _store = new();

    public Task AddAsync(T entity)
    {
        _store.Add(entity);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<T>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<T>>(_store);
    }

    public Task<T?> GetByIdAsync(object id)
    {
        // Burada reflection veya custom id mantığı eklenebilir
        return Task.FromResult<T?>(null);
    }

    public Task UpdateAsync(T entity)
    {
        // In-memory için basit implementasyon
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity)
    {
        _store.Remove(entity);
        return Task.CompletedTask;
    }
}

// Unit of Work implementasyonu
public class UnitOfWork : IUnitOfWork
{
    private readonly Dictionary<Type, object> _repositories = new();

    public IRepository<T> Repository<T>() where T : class
    {
        if (!_repositories.ContainsKey(typeof(T)))
        {
            _repositories[typeof(T)] = new InMemoryRepository<T>();
        }
        return (IRepository<T>)_repositories[typeof(T)];
    }

    public Task<int> CommitAsync()
    {
        // In-memory için commit sayısı = repository sayısı
        return Task.FromResult(_repositories.Count);
    }

    public void Rollback()
    {
        _repositories.Clear();
    }

    public void Dispose()
    {
        _repositories.Clear();
    }
}