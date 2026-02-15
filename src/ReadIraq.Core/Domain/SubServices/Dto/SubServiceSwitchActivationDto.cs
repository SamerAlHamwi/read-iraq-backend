using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Domain.SubServices.Dto
{
    public class SubServiceSwitchActivationDto
    {
        [Required]
        public int Id { get; set; }
    }
}
