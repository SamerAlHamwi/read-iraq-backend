using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace ReadIraq.Points.Dto
{
    public class LitePointDto : EntityDto<int>
    {

        public string Name { get; set; }
        public int NumberOfPoint { get; set; }
        public double Price { get; set; }
        public List<PointTranslationDto> Translations { get; set; }
        public bool IsActive { get; set; }
        public int NumberInMonths { get; set; }
        public bool IsForFeature { get; set; }

    }


}
