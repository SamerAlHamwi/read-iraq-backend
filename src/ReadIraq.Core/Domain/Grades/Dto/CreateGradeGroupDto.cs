using ReadIraq.Domain.Translations.Dto;
using System.Collections.Generic;

namespace ReadIraq.Domain.Grades.Dto
{
    public class CreateGradeGroupDto
    {
        public List<CreateTranslationDto> Name { get; set; }
        public int Priority { get; set; }
        public string Description { get; set; }
    }
}
