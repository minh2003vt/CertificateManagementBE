using Application.IRepositories;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class CertificateTemplateRepository : GenericRepository<CertificateTemplate>, ICertificateTemplateRepository
    {
        public CertificateTemplateRepository(Context context) : base(context)
        {
        }
    }
}


