using CarRental.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CarRental.Services
{
    /// <summary>
    /// Base generic service for common CRUD operations
    /// </summary>
    public class GenericService<T> : IGenericService<T> where T : class
    {
        protected readonly CarRentalContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericService(CarRentalContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }
            //Tüm kayıtları getir → SELECT * FROM tablo
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
//INSERT INTO
        public virtual async Task<T> CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
                return false;

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
    }
}
