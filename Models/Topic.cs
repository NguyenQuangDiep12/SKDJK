using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SKDJK.Models
{
    public class Topic
    {
        public int Id { get; set; }
        public int LanguageId { get; set; }
        public Language Language { get; set; } = null!;
        public string Name { get; set; }
        public string Level { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}