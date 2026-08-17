namespace SKDJK.Dtos.Home
{
    public class SuggestedTopicDto
    {
        public int TopicId { get; init; }

        public string Name { get; init; } = string.Empty;

        public string Level { get; init; } = string.Empty;

        public string? Description { get; init; }

        public string? ImageUrl { get; init; }
    }
}