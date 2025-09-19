using Application.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected DbSet<T> _dbSet;
        protected readonly Context _context;
        public GenericRepository(Context context)
        {
            this._context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IList<T>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T?> GetSingleOrDefaultByNullableExpressionAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null)
                return null;

            return await _context.Set<T>().SingleOrDefaultAsync(predicate);
        }

        public async Task<IList<T>> GetByNullableExpressionWithOrderingAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
        {
            predicate ??= p => true;
            var query = _context.Set<T>().Where(predicate);
            return await (orderBy != null ?  orderBy(query) : query).ToListAsync();
        }

        public async Task<IList<T>> GetAllAsNoTracking()
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetSingleOrDefaultByNullableExpressionAsNoTrackingAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null)
                return null;

            return await _context.Set<T>().AsNoTracking().SingleOrDefaultAsync(predicate);
        }

        public async Task<IList<T>> GetByNullableExpressionWithOrderingAsNoTrackingAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
        {
            predicate ??= p => true;
            var query = _context.Set<T>().AsNoTracking().Where(predicate);
            return await (orderBy != null ? orderBy(query) : query).ToListAsync();
        }

        public async Task<int> CountByNullableExpressionAsync(Expression<Func<T, bool>>? predicate = null)
        {
            predicate ??= p => true;
            return await _context.Set<T>().CountAsync(predicate);
        }
        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteByNullableExpressionAsync(Expression<Func<T, bool>>? predicate = null)
        {
            predicate ??= p => true;
            var entitiesToDelete = _context.Set<T>().Where(predicate);
            _context.Set<T>().RemoveRange(entitiesToDelete);
            return await _context.SaveChangesAsync();
        }
    }
}
