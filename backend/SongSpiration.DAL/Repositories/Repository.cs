using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SongSpiration.DAL.Interfaces;

namespace SongSpiration.DAL.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly SongSpirationDbContext _db;
    protected readonly DbSet<T> _dbSet;

    public Repository(SongSpirationDbContext db)
    {
        _db = db;
        _dbSet = db.Set<T>();
    }

    public virtual async Task AddAsync(T entity) => await _dbSet.AddAsync(entity).AsTask();

    public virtual async Task<IEnumerable<T>> GetAllAsync()
        => await _dbSet.AsNoTracking().ToListAsync();

    public virtual async Task<T?> GetByIdAsync(Guid id)
        => await _dbSet.FindAsync(id);

    public virtual void Remove(T entity) => _dbSet.Remove(entity);

    public virtual void Update(T entity) => _dbSet.Update(entity);

    public virtual Task<int> SaveChangesAsync() => _db.SaveChangesAsync();
}