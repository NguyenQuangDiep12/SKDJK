namespace SKDJK.Dtos.Test
{
    public class QuestionDto
    {
        public int Id { get; init; }

        public string Content { get; init; } = string.Empty;

        public string QuestionType { get; init; } = string.Empty;

        public string? ImageUrl { get; init; }

        public string? AudioUrl { get; init; }

        public List<AnswerDto> Answers { get; init; } = [];
    }
}