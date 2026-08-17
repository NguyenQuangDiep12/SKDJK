namespace SKDJK.Dtos.Test
{
    public class TestDetailDto
    {
        public int Id { get; init; }

        public string Title { get; init; } = string.Empty;

        public string? Description { get; init; }

        public string Level { get; init; } = string.Empty;

        public int DurationMinutes { get; init; }

        public List<QuestionDto> Questions { get; init; } = [];
    }
}