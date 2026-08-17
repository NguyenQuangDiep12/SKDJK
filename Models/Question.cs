using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SKDJK.Models.enums;

namespace SKDJK.Models
{
    public class Question
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public Test Test { get; set; } = null!;
        public string Content { get; set; }
        public QuestionType QuestionType { get; set; }
        public string? ImageUrl { get; set; }
        public string? AudioUrl { get; set; }
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}