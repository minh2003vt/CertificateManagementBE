using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PlanCertificate
    {
        [ForeignKey("Certificate")]
        public string CertificateId { get; set; } = string.Empty;
        public virtual Certificate Certificate { get; set; } = null!;
        [ForeignKey("Plan")]
        public string PlanId { get; set; } = string.Empty;
        public virtual Plan Plan { get; set; } = null!;
    }
}
