namespace SKDJK.Dtos.Topic
{
    public class TopicListItemDto
    {
        public int Id { get; init; }

        public string Name { get; init; } = string.Empty;

        public string Level { get; init; } = string.Empty;

        public int LanguageId { get; init; }

        public string LanguageName { get; init; } = string.Empty;

        public string? ImageUrl { get; init; }

        public int LessonCount { get; init; }
    }
}