using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SKDJK.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
        public ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
        public ICollection<LearningProgress> LearningProgresses { get; set; } = new List<LearningProgress>();
        public ICollection<PronunciationResult> PronunciationResults { get; set; } = new List<PronunciationResult>();
    }
}