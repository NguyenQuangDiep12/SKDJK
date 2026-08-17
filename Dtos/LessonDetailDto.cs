namespace SKDJK.Dtos.Lesson
{
    public class LessonDetailDto
    {
        public int Id { get; init; }

        public int TopicId { get; init; }

        public string TopicName { get; init; } = string.Empty;

        public string Title { get; init; } = string.Empty;

        public string? Description { get; init; }

        public string Content { get; init; } = string.Empty;

        public decimal CompletionPercent { get; init; }

        public List<VocabularyDto> Vocabularies { get; init; } = [];

        public List<LessonSectionDto> Sections { get; init; } = [];
    }
}