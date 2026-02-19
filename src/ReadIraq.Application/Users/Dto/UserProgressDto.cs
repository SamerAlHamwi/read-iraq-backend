using ReadIraq.Grades.Dto;
using ReadIraq.Subjects.Dto;
using System.Collections.Generic;

namespace ReadIraq.Users.Dto
{
    public class UserProgressDto
    {
        public long UserId { get; set; }
        public int TotalPoints { get; set; }
        public GradeDto Grade { get; set; }
        public List<SubjectProgressDto> SubjectProgresses { get; set; }

        public UserProgressDto()
        {
            SubjectProgresses = new List<SubjectProgressDto>();
        }
    }

    public class SubjectProgressDto
    {
        public LiteSubjectDto Subject { get; set; }
        public decimal ProgressPercentage { get; set; }
    }
}
