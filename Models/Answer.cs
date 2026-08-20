using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SKDJK.Models
{
    public class Answer
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;
        public string Content { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }
        public ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
    }
}
