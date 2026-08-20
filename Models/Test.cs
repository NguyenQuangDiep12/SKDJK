using SKDJK.Models.enums;

namespace SKDJK.Models
{
    // Đại diện một đề Practice hoặc FullMock thuộc một bài học.
    public class Test
    {
        // Khóa chính của Test.
        public int Id { get; set; }
        // Khóa ngoại tới Lesson để suy ra Topic và Level.
        public int LessonId { get; set; }
        // Navigation tới bài học chứa Test.
        public Lesson Lesson { get; set; } = null!;
        // Tên đề hiển thị ở danh sách và kết quả.
        public string Title { get; set; } = string.Empty;
        // Số phút dùng cho đồng hồ đếm ngược.
        public int DurationMinutes { get; set; }
        // Mô tả ngắn có thể để trống.
        public string? Description { get; set; }
        // Format quyết định quy tắc Part/Section.
        public TestFormat Format { get; set; } = TestFormat.ToeicStyle;
        // Mode quyết định Practice hay kiểm tra cấu trúc FullMock.
        public TestMode Mode { get; set; } = TestMode.Practice;
        // Chỉ đề đang hoạt động mới xuất hiện và nhận bài nộp.
        public bool IsActive { get; set; } = true;
        // Lưu thời điểm tạo đề theo UTC.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Danh sách câu thuộc đề.
        public ICollection<Question> Questions { get; set; } = new List<Question>();
        // Danh sách lượt làm dùng để khóa cấu trúc khi đã có lịch sử.
        public ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
    }
}
