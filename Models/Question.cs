using SKDJK.Models.enums;

namespace SKDJK.Models
{
    // Đại diện một câu hỏi và metadata cần để group media hoặc passage.
    public class Question
    {
        // Khóa chính của câu hỏi.
        public int Id { get; set; }
        // Khóa ngoại tới đề chứa câu.
        public int TestId { get; set; }
        // Navigation tới Test để validation biết Format và Mode.
        public Test Test { get; set; } = null!;
        // Nội dung câu hiển thị cho người học.
        public string Content { get; set; } = string.Empty;
        // Loại câu quyết định render radio hay input text.
        public QuestionType QuestionType { get; set; }
        // Section chỉ nhận Listening hoặc Reading.
        public string SectionName { get; set; } = "Reading";
        // Số Part TOEIC/IELTS hoặc Passage IELTS Reading.
        public int PartNumber { get; set; } = 1;
        // Thứ tự duy nhất trong cùng Test.
        public int Order { get; set; }
        // Passage, email hoặc notice dùng chung cho group Reading.
        public string? ContextText { get; set; }
        // Mã conversation, recording hoặc passage để gom nhiều câu.
        public string? GroupCode { get; set; }
        // Ảnh minh họa, bắt buộc với TOEIC Part 1.
        public string? ImageUrl { get; set; }
        // Recording, bắt buộc với các câu Listening.
        public string? AudioUrl { get; set; }
        // Hướng dẫn làm câu, gồm cả câu chữ word-limit IELTS.
        public string? Instruction { get; set; }
        // Số từ tối đa của câu text; null nghĩa là không giới hạn.
        public int? MaxWords { get; set; }
        // Các option hoặc chuỗi đáp án chấp nhận được.
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
        // Các câu trả lời lịch sử tham chiếu Question này.
        public ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
    }
}
