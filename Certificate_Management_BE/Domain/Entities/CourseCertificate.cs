using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CourseCertificate
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Certificate")]
        public string CertificateId { get; set; } = string.Empty;
        public virtual Certificate Certificate { get; set; } = null!;

        [Key, Column(Order = 1)]
        [ForeignKey("Course")]
        public string CourseId { get; set; } = string.Empty;
        public virtual Course Course { get; set; } = null!;
    }
}
