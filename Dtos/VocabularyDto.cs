namespace SKDJK.Dtos.Lesson
{
    public class VocabularyDto
    {
        public int Id { get; init; }

        public string Word { get; init; } = string.Empty;

        public string? Meaning { get; init; }

        public string? Pronunciation { get; init; }

        public string? Example { get; init; }

        public string? AudioUrl { get; init; }
    }
}