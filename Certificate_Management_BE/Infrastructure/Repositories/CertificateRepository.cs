using Application.IRepositories;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class CertificateRepository : GenericRepository<Certificate>, ICertificateRepository
    {
        public CertificateRepository(Context context) : base(context)
        {
        }
    }
}


