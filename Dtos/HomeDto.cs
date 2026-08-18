namespace SKDJK.Dtos;

public class HomeDto
{
    public int LearnedTopicCount { get; init; }
    public int CompletedTestCount { get; init; }
    public decimal OverallProgress { get; init; }
    public ContinueLearningDto? ContinueLearning { get; init; }
    public List<SuggestedTopicDto> SuggestedTopics { get; init; } = [];
}
public class ContinueLearningDto
{
    public int LessonId { get; init; }
    public int TopicId { get; init; }
    public string TopicName { get; init; } = string.Empty;
    public string LessonTitle { get; init; } = string.Empty;
    public decimal CompletionPercent { get; init; }
}
public class SuggestedTopicDto
{
    public int TopicId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Level { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? ImageUrl { get; init; }
}
