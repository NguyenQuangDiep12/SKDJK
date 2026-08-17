using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SKDJK.Models
{
    public class Test
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; } = null!;
        public string Title { get; set; }
        public string? Description { get; set; }
        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
    }
}