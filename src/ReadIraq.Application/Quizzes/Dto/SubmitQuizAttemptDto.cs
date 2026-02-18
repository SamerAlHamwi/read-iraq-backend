using System;
using System.Collections.Generic;

namespace ReadIraq.Quizzes.Dto
{
    public class SubmitQuizAttemptDto
    {
        public Guid QuizId { get; set; }
        public List<UserAnswerDto> Answers { get; set; }
    }

    public class UserAnswerDto
    {
        public Guid QuestionId { get; set; }
        public string Answer { get; set; }
    }
}
