using System.ComponentModel.DataAnnotations;

namespace SKDJK.Models
{
    public class Language
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ICollection<Topic> Topics { get; set; } = new List<Topic>();
    }
}
