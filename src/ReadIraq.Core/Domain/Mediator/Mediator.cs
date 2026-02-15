using Abp.Domain.Entities.Auditing;
using ReadIraq.Domain.Cities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.Mediators
{
    public class Mediator : FullAuditedEntity, IActiveEntity
    {
        [Required]
        [StringLength(8)]
        public string MediatorCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string CompanyPhoneNumber { get; set; }
        public string MediatorPhoneNumber { get; set; }
        public string DialCode { get; set; }
        public double CommissionPercentage { get; set; }
        public bool IsActive { get; set; }
        public int? CityId { get; set; }
        [ForeignKey(nameof(CityId))]
        public virtual City City { get; set; }
        public string Email { get; set; }
        public double MoneyOwed { get; set; }
    }
}
