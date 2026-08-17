using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SKDJK.Models.enums;

namespace SKDJK.Models
{
    public class LearningProgress
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; } = null!;
        public LearningStatus Status { get; set; }
        public decimal CompletionPercent { get; set; }
        public DateTime? LastStudyAt { get; set; }
    }
}