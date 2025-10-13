using Application.IRepositories;
using Domain.Entities;
using Infrastructure;

namespace Infrastructure.Repositories
{
    public class RequestEntityRepository : GenericRepository<RequestEntity>, IRequestEntityRepository
    {
        public RequestEntityRepository(Context context) : base(context)
        {
        }
    }
}
