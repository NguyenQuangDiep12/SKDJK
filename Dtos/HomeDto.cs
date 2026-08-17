namespace SKDJK.Dtos.Home
{
    public class HomeDto
    {
        public int LearnedTopicCount { get; init; }

        public int CompletedTestCount { get; init; }

        public decimal OverallProgress { get; init; }

        public ContinueLearningDto? ContinueLearning { get; init; }

        public List<SuggestedTopicDto> SuggestedTopics { get; init; } = [];
    }
}