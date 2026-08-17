using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SKDJK.Models
{
    public class Lesson
    {
        public int Id { get; set; }
        public int TopicId { get; set; }
        public Topic Topic { get; set; } = null!;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Content { get; set; }
        public ICollection<Vocabulary> Vocabularies { get; set; } = new List<Vocabulary>();
        public ICollection<Test> Tests { get; set; } = new List<Test>();
        public ICollection<LearningProgress> LearningProgresses { get; set; } = new List<LearningProgress>();
    }
}