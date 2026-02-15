using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace ReadIraq.Points.Dto
{
    public class PointDetailsDto : EntityDto
    {
        public string Name { get; set; }
        public int NumberOfPoint { get; set; }
        public double Price { get; set; }
        public bool IsActive { get; set; }
        public List<PointTranslationDto> Translations { get; set; }
        public int NumberInMonths { get; set; }
        public bool IsForFeature { get; set; }

    }
}
