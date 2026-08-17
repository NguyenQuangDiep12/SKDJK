using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SKDJK.Models
{
    public class PronunciationResult
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int VocabularyId { get; set; }
        public Vocabulary Vocabulary { get; set; } = null!;
        public decimal Score { get; set; }
    }
}