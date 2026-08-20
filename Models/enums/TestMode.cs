namespace SKDJK.Models.enums
{
    // Phân biệt bài luyện rút gọn và bài mô phỏng đầy đủ theo cấu trúc công khai.
    public enum TestMode
    {
        // Practice cho phép ít câu hơn nhưng vẫn giữ đúng Part/Section và loại câu hỏi.
        Practice = 1,

        // FullMock bắt buộc đủ số câu và thời gian theo format đã chọn.
        FullMock = 2
    }
}
