using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Domain.Partners.Dto
{
    public class UpdatePartnerDto : CreatePartnerDto, IEntityDto
    {
        [Required]
        public int Id { get; set; }
    }
}
