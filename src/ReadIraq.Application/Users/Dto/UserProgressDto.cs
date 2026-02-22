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
        public List<UserSubjectProgressDto> SubjectProgresses { get; set; }

        public UserProgressDto()
        {
            SubjectProgresses = new List<UserSubjectProgressDto>();
        }
    }

    public class UserSubjectProgressDto
    {
        public LiteSubjectDto Subject { get; set; }
        public decimal ProgressPercentage { get; set; }
    }
}
