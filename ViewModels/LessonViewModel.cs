using SKDJK.Models.enums;

namespace SKDJK.ViewModels
{
    public class LessonStudyViewModel
    {
        public int Id { get; set; }

        public int TopicId { get; set; }

        public string TopicName { get; set; } = string.Empty;

        public string TopicLevel { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Content { get; set; } = string.Empty;

        public decimal CompletionPercent { get; set; }

        public List<VocabularyItemViewModel> Vocabularies { get; set; } = new();

        public List<LessonSectionViewModel> GrammarSections { get; set; } = new();

        public List<LessonSectionViewModel> ListeningSections { get; set; } = new();

        public List<LessonSectionViewModel> SpeakingSections { get; set; } = new();

        public List<LessonTestLinkViewModel> Tests { get; set; } = new();
    }

    public class VocabularyItemViewModel
    {
        public int Id { get; set; }

        public string Word { get; set; } = string.Empty;

        public string? Meaning { get; set; }

        public string? Pronunciation { get; set; }

        public string? Example { get; set; }

        public string? AudioUrl { get; set; }
    }

    public class LessonSectionViewModel
    {
        public int Id { get; set; }

        public LessonSectionType Type { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string? AudioUrl { get; set; }

        public int SortOrder { get; set; }
    }

    public class LessonTestLinkViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
    }
}
