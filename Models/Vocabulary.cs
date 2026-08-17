using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SKDJK.Models
{
    public class Vocabulary
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }
        public string Word { get; set; }
        public string Meaning { get; set; }
        public string Pronunciation { get; set; }
        public string Example { get; set; }
        public string? AudioUrl { get; set; }
        public ICollection<PronunciationResult> PronunciationResults { get; set; } = new List<PronunciationResult>();
    }
}