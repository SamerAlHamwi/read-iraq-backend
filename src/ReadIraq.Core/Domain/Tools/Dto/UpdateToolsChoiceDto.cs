using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Domain.Toolss.Dto
{
    public class UpdateToolDto : CreateToolDto, IEntityDto
    {
        [Required]
        public int Id { get; set; }
    }
}
