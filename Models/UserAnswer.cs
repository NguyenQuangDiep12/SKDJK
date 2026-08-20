namespace SKDJK.Models
{
    // Lưu câu trả lời đã chấm cho đúng một Question trong một TestResult.
    public class UserAnswer
    {
        // Khóa chính của dòng trả lời.
        public int Id { get; set; }

        // Khóa ngoại tới lượt làm.
        public int TestResultId { get; set; }

        // Khóa ngoại tới câu hỏi.
        public int QuestionId { get; set; }

        // Chỉ câu lựa chọn lưu AnswerId; câu text luôn để null.
        public int? AnswerId { get; set; }

        // Chỉ câu text lưu nội dung; câu lựa chọn luôn để null.
        public string? TextAnswer { get; set; }

        // Snapshot kết quả đúng/sai do server tính.
        public bool IsCorrect { get; set; }

        // Navigation tới kết quả cha.
        public TestResult TestResult { get; set; } = null!;

        // Navigation tới câu hỏi được trả lời.
        public Question Question { get; set; } = null!;

        // Navigation nullable vì câu text không chọn Answer.
        public Answer? Answer { get; set; }
    }
}
