using Application.IRepositories;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class SubjectCertificateRepository : GenericRepository<SubjectCertificate>, ISubjectCertificateRepository
    {
        public SubjectCertificateRepository(Context context) : base(context)
        {
        }
    }
}


