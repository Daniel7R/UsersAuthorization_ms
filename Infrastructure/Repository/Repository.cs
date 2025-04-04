﻿using Microsoft.EntityFrameworkCore;
using UsersAuthorization.Infrastructure.Data;

namespace UsersAuthorization.Infrastructure.Repository
{
    public class Repository<T>: IRepository<T> where T : class
    {
        private readonly UserDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(UserDbContext context)
        {
            _context= context;
            _dbSet= context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);

            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }

        }

        public Task DeleteAsync(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
