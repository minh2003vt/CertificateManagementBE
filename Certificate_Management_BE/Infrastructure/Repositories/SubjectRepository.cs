using Application.IRepositories;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class SubjectRepository : GenericRepository<Subject>, ISubjectRepository
    {
        public SubjectRepository(Context context) : base(context)
        {
        }
    }
}


