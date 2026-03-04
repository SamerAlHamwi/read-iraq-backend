using System;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.ActivationCodes.Dto
{
    public class UseCodeInput
    {
        [Required]
        public string Code { get; set; }

        public Guid? SubjectId { get; set; }
    }
}
