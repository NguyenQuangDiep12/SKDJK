using System.ComponentModel.DataAnnotations;

namespace SKDJK.Models
{
    public class Language
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }
        public ICollection<Topic> Topics { get; set; } = new List<Topic>();
    }
}