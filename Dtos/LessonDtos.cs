namespace SKDJK.Dtos
{
   public class VocabularyLearningDto
   {
        public int LessonId { get; init; }

        public string LessonTitle { get; init; } = string.Empty;

        public List<VocabularyItemDto> Vocabularies { get; init; } = [];
   }
    public class VocabularyItemDto
    {
        public int VocabularyId { get; init; }

        public string Word { get; init; } = string.Empty;

        public string Meaning { get; init; } = string.Empty;

        public string? Pronunciation { get; init; }

        public string? Example { get; init; }

        public string? AudioUrl { get; init; }
    }
}
