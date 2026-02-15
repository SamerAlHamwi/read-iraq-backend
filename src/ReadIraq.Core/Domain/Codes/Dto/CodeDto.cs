using Abp.Application.Services.Dto;
using System.Collections.Generic;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.Codes.Dto
{
    public class CodeDto : EntityDto
    {
        public string RSMCode { get; set; }

        public decimal DiscountPercentage { get; set; }
        public List<string> PhonesNumbers { get; set; }
        public bool IsActive { get; set; }
        public CodeType CodeType { get; set; }
    }
}
