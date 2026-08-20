namespace SKDJK.Dtos
{
    // DTO màn quản trị Lesson/Vocabulary hai cột.
    public sealed class AdminLessonPageDto
    {
        public int? SelectedLessonId { get; set; }
        public AdminLessonItemDto? SelectedLesson { get; set; }
        public List<AdminLessonItemDto> Lessons { get; set; } = [];
        public List<AdminVocabularyItemDto> Vocabularies { get; set; } = [];
    }

    // DTO một bài học trên bảng quản trị.
    public sealed class AdminLessonItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string TopicName { get; set; } = string.Empty;
    }

    // DTO một từ vựng trên bảng quản trị.
    public sealed class AdminVocabularyItemDto
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public string Word { get; set; } = string.Empty;
        public string? Meaning { get; set; }
        public string? Pronunciation { get; set; }
    }

    // DTO form Lesson chuyển hai chiều giữa Controller và Service.
    public sealed class AdminLessonFormDto
    {
        public int? Id { get; set; }
        public int TopicId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Content { get; set; } = string.Empty;
        public List<AdminTopicOptionDto> Topics { get; set; } = [];
    }

    // DTO option Topic cho form Lesson.
    public sealed class AdminTopicOptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    // DTO form Vocabulary chuyển hai chiều giữa Controller và Service.
    public sealed class AdminVocabularyFormDto
    {
        public int? Id { get; set; }
        public int LessonId { get; set; }
        public string Word { get; set; } = string.Empty;
        public string? Meaning { get; set; }
        public string? Pronunciation { get; set; }
        public string? Example { get; set; }
        public string? AudioUrl { get; set; }
    }
}
