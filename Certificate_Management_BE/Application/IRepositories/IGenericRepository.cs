using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IList<T>> GetAll();
        Task<T?> GetSingleOrDefaultByNullableExpressionAsync(Expression<Func<T, bool>>? predicate = null);
        Task<IList<T>> GetByNullableExpressionWithOrderingAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);
        Task<IList<T>> GetAllAsNoTracking();
        Task<T?> GetSingleOrDefaultByNullableExpressionAsNoTrackingAsync(Expression<Func<T, bool>>? predicate = null);
        Task<IList<T>> GetByNullableExpressionWithOrderingAsNoTrackingAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);
        Task<int> CountByNullableExpressionAsync(Expression<Func<T, bool>>? predicate = null);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<int> DeleteByNullableExpressionAsync(Expression<Func<T, bool>>? predicate = null);
    }
}
