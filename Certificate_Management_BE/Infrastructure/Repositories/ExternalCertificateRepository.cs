using Application.IRepositories;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class ExternalCertificateRepository : GenericRepository<ExternalCertificate>, IExternalCertificateRepository
    {
        public ExternalCertificateRepository(Context context) : base(context)
        {
        }
    }
}


