namespace SKDJK.Dtos
{
    public class LessonListItemDto
    {
        public int Id { get; init; }

        public string Title { get; init; } = string.Empty;

        public string? Description { get; init; }

        public string Level { get; init; } = string.Empty;

        public decimal CompletionPercent { get; init; }

        public bool IsCompleted { get; init; }
    }
}