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

        public List<TopicCardViewModel> SuggestedTopics { get; set; } = new();
    }

    public class ContinueLessonViewModel
    {
        public int LessonId { get; set; }

        public string TopicName { get; set; } = string.Empty;

        public string LessonTitle { get; set; } = string.Empty;

        public decimal CompletionPercent { get; set; }
    }
}
