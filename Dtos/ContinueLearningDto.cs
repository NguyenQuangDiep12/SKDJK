namespace SKDJK.Dtos.Home
{
    public class ContinueLearningDto
    {
        public int LessonId { get; init; }

        public int TopicId { get; init; }

        public string TopicName { get; init; } = string.Empty;

        public string LessonTitle { get; init; } = string.Empty;

        public decimal CompletionPercent { get; init; }
    }
}