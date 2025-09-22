using Application.IRepositories;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class CourseCertificateRepository : GenericRepository<CourseCertificate>, ICourseCertificateRepository
    {
        public CourseCertificateRepository(Context context) : base(context)
        {
        }
    }
}


