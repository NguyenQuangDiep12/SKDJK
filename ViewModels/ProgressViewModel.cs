namespace SKDJK.ViewModels
{
    public class ProgressViewModel
    {
        public int CompletedLessonCount { get; set; }

        public int TotalLessonCount { get; set; }

        public int CompletedTestCount { get; set; }

        public int TotalTestCount { get; set; }

        public decimal OverallProgress { get; set; }

        public List<TopicProgressViewModel> TopicProgresses { get; set; } = new();

        public List<CompletedLessonViewModel> CompletedLessons { get; set; } = new();

        public List<TestHistoryItemViewModel> TestHistory { get; set; } = new();
    }

    public class TopicProgressViewModel
    {
        public int TopicId { get; set; }

        public string TopicName { get; set; } = string.Empty;

        public decimal ProgressPercent { get; set; }
    }

    public class CompletedLessonViewModel
    {
        public int LessonId { get; set; }

        public string LessonTitle { get; set; } = string.Empty;

        public string TopicName { get; set; } = string.Empty;

        public DateTime? CompletedAt { get; set; }
    }
}
