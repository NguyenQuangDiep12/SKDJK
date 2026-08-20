namespace SKDJK.Dtos
{
    // DTO tổng hợp tiến độ từ Service sang Controller.
    public sealed class ProgressDto
    {
        public int CompletedLessonCount { get; set; }
        public int TotalLessonCount { get; set; }
        public int CompletedTestCount { get; set; }
        public int TotalTestCount { get; set; }
        public decimal OverallProgress { get; set; }
        public List<TopicProgressDto> TopicProgresses { get; set; } = [];
        public List<CompletedLessonDto> CompletedLessons { get; set; } = [];
        public List<TestHistoryItemDto> TestHistory { get; set; } = [];
    }

    // DTO tiến độ của một chủ đề.
    public sealed class TopicProgressDto
    {
        public int TopicId { get; set; }
        public string TopicName { get; set; } = string.Empty;
        public decimal ProgressPercent { get; set; }
    }

    // DTO một bài học đã hoàn thành.
    public sealed class CompletedLessonDto
    {
        public int LessonId { get; set; }
        public string LessonTitle { get; set; } = string.Empty;
        public string TopicName { get; set; } = string.Empty;
        public DateTime? CompletedAt { get; set; }
    }
}
