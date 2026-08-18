using SKDJK.Models.enums;

namespace SKDJK.ViewModels
{
    public class VocabularyLearningViewModel
    {
        public int LessonId { get; set; }
        public string LessonTitle { get; set; }
        public int LessonTotal => Items.Count;
        public List<VocabularyItemViewModel> Items { get; set; }
    }
    public class VocabularyItemViewModel
    {
        public int VocabularyId { get; set; }
        public string Word { get; set; }
        public string? Meaning { get; set; }
        public string? Pronunciation { get; set; }
        public string? Example { get; set; }
        public string? AudioUrl { get; set; }
    }
}
