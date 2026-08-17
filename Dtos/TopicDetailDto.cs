using SKDJK.Dtos;

namespace SKDJK.Dtos
{
    public class TopicDetailDto
    {
        public int Id { get; init; }

        public string Name { get; init; } = string.Empty;

        public string Level { get; init; } = string.Empty;

        public string? Description { get; init; }

        public string? ImageUrl { get; init; }

        public int LanguageId { get; init; }

        public string LanguageName { get; init; } = string.Empty;

        public List<LessonListItemDto> Lessons { get; init; } = [];
    }
}