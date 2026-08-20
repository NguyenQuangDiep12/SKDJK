namespace SKDJK.Models
{
    // Lưu một lượt nộp bài đã được server chấm.
    public class TestResult
    {
        // Khóa chính của lượt làm.
        public int Id { get; set; }
        // Người dùng sở hữu kết quả.
        public int UserId { get; set; }
        // Navigation tới người dùng.
        public User User { get; set; } = null!;
        // Test được làm tại thời điểm nộp.
        public int TestId { get; set; }
        // Navigation tới Test để hiển thị metadata.
        public Test Test { get; set; } = null!;
        // Phần trăm nội bộ SKDJK đã làm tròn hai chữ số.
        public decimal Score { get; set; }
        // Số câu đúng do server tính.
        public int CorrectCount { get; set; }
        // Snapshot tổng câu để lịch sử không phụ thuộc đề hiện tại.
        public int TotalQuestions { get; set; }
        // Thời điểm hoàn tất theo UTC.
        public DateTime? SubmittedAt { get; set; }
        // Chi tiết từng câu của lượt làm.
        public ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
    }
}
