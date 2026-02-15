using Abp.Application.Services.Dto;
using ReadIraq.Cities.Dto;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Mediators.Dto
{
    public class MediatorDetailsDto : EntityDto
    {
        [Required]
        [StringLength(8)]
        public string MediatorCode { get; set; }
        public string MediatorPhoneNumber { get; set; }
        public string DialCode { get; set; }
        public decimal CommissionPercentage { get; set; }
        public decimal MediatorProfit { get; set; }
        public bool IsActive { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string CompanyPhoneNumber { get; set; }
        public string Email { get; set; }
        public LiteCityDto City { get; set; }
        public int Points { get; set; }
        public int CountRegisteredUsers { get; set; }
        public int NumberServiceUsers { get; set; }
        public double MoneyOwed { get; set; }


    }
}
