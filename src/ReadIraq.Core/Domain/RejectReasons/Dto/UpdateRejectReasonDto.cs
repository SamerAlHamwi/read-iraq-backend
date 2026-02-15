
using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Domain.RejectReasons.Dto
{
    public class UpdateRejectReasonDto : CreateRejectReasonDto, IEntityDto
    {
        public int Id { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (Id is 0)
                context.Results.Add(new ValidationResult(" Id Must not Be 0 "));
        }
    }
}
