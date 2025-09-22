using Application.IRepositories;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class RequestRepository : GenericRepository<Request>, IRequestRepository
    {
        public RequestRepository(Context context) : base(context)
        {
        }
    }
}


