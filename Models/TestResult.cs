using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SKDJK.Models
{
    public class TestResult
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int TestId { get; set; }
        public Test Test { get; set; } = null!;
        public decimal Score { get; set; }
        public int CorrectCount { get; set; }
        public DateTime? SubmittedAt { get; set; }
    }
}