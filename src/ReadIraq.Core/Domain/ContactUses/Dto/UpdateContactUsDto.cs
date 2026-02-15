using Abp.Application.Services.Dto;

namespace ReadIraq.ContactUsService.Dto
{
    public class UpdateContactUsDto : CreateContactUsDto, IEntityDto
    {

        public int Id { get; set; }



    }
}
