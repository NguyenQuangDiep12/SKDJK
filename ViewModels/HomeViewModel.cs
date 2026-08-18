namespace SKDJK.ViewModels
{
    public class HomeViewModel
    {
        public bool IsAuthenticated { get; set; }

        public string FullName { get; set; } = "Người học";

        public int LearnedTopicCount { get; set; }

        public int CompletedTestCount { get; set; }

        public decimal OverallProgress { get; set; }

        public ContinueLessonViewModel? ContinueLesson { get; set; }

        public List<SuggestedTopicViewModel> SuggestedTopics { get; set; } = new();
    }

    public class ContinueLessonViewModel
    {
        public int LessonId { get; set; }

        public string TopicName { get; set; } = string.Empty;

        public string LessonTitle { get; set; } = string.Empty;

        public decimal CompletionPercent { get; set; }
    }
    public class SuggestedTopicViewModel
    {
        public int TopicId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Level { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }
    }
}
