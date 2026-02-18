using System.Collections.Generic;

namespace ReadIraq.Grades.Dto
{
    public class ReorderGradesDto
    {
        public List<GradeOrderDto> Orders { get; set; }
    }

    public class GradeOrderDto
    {
        public int Id { get; set; }
        public int Priority { get; set; }
    }
}
