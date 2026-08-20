namespace SKDJK.Dtos
{
    // DTO danh sách chủ đề dùng ở ranh giới Service và Controller.
    public sealed class TopicListDto
    {
        public string? Search { get; set; }
        public int? LanguageId { get; set; }
        public string? Level { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public int TotalItems { get; set; }
        public List<LanguageOptionDto> Languages { get; set; } = [];
        public List<string> Levels { get; set; } = [];
        public List<TopicCardDto> Topics { get; set; } = [];
    }

    // DTO dữ liệu một chủ đề trong danh sách.
    public sealed class TopicCardDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string LanguageName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }

    // DTO option ngôn ngữ cho Controller ánh xạ thành option ViewModel.
    public sealed class LanguageOptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    // DTO chi tiết chủ đề và các bài học thuộc chủ đề.
    public sealed class TopicDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string LanguageName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public List<TopicLessonDto> Lessons { get; set; } = [];
    }

    // DTO một bài học trong trang chi tiết chủ đề.
    public sealed class TopicLessonDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal CompletionPercent { get; set; }
    }

    // DTO danh sách chủ đề quản trị.
    public sealed class AdminTopicListDto
    {
        public string? Search { get; set; }
        public int? LanguageId { get; set; }
        public string? Level { get; set; }
        public List<LanguageOptionDto> Languages { get; set; } = [];
        public List<TopicCardDto> Items { get; set; } = [];
    }

    // DTO form chủ đề phục vụ riêng cho Razor tạo/sửa và danh sách ngôn ngữ.
    public sealed class AdminTopicFormDto
    {
        public int? Id { get; set; }
        public int LanguageId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public List<LanguageOptionDto> Languages { get; set; } = [];
    }

    // DTO lệnh tạo Topic không có Id vì database chịu trách nhiệm sinh khóa chính.
    public sealed class CreateTopicDto
    {
        public int LanguageId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }

    // DTO lệnh cập nhật không có Id; Controller truyền Id URL riêng vào UpdateAsync.
    public sealed class UpdateTopicDto
    {
        public int LanguageId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }
}
