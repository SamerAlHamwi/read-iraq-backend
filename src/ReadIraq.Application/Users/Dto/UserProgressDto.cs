using System.Collections.Generic;

namespace ReadIraq.Users.Dto
{
    public class UserProgressDto
    {
        public long UserId { get; set; }
        public int TotalPoints { get; set; }
        public double CompletionPercentage { get; set; }
        // Add more fields as needed
    }
}
