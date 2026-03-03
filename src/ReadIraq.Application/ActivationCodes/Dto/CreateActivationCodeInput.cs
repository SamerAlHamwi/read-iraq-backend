using System;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.ActivationCodes.Dto
{
    public class CreateActivationCodeInput
    {
        public Guid? SubjectId { get; set; }
        public Guid? TeacherId { get; set; }
        public int? GradeId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(1, 1000)]
        public int Count { get; set; } = 1;
    }
}
