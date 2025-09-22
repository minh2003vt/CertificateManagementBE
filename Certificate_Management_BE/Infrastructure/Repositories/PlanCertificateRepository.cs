using Application.IRepositories;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class PlanCertificateRepository : GenericRepository<PlanCertificate>, IPlanCertificateRepository
    {
        public PlanCertificateRepository(Context context) : base(context)
        {
        }
    }
}


