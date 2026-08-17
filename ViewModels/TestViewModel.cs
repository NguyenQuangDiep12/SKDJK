using SKDJK.Models.enums;
using System.ComponentModel.DataAnnotations;

namespace SKDJK.ViewModels
{
    public class TestListViewModel
    {
        public string? Search { get; set; }

        public string? Level { get; set; }

        public List<string> Levels { get; set; } = new();

        public List<TestListItemViewModel> Tests { get; set; } = new();
    }

    public class TestListItemViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string LessonTitle { get; set; } = string.Empty;

        public string TopicName { get; set; } = string.Empty;

        public string Level { get; set; } = string.Empty;

        public int QuestionCount { get; set; }

        public int DurationMinutes { get; set; }

        public decimal? BestScore { get; set; }

        public int AttemptCount { get; set; }
    }

    public class TakeTestViewModel
    {
        public int TestId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string LessonTitle { get; set; } = string.Empty;

        public string TopicName { get; set; } = string.Empty;

        public string Level { get; set; } = string.Empty;

        public int DurationMinutes { get; set; }

        public List<TestQuestionViewModel> Questions { get; set; } = new();
    }

    public class TestQuestionViewModel
    {
        public int Id { get; set; }

        public string Content { get; set; } = string.Empty;

        public QuestionType QuestionType { get; set; }

        public string? ImageUrl { get; set; }

        public string? AudioUrl { get; set; }

        public List<TestAnswerOptionViewModel> Answers { get; set; } = new();
    }

    public class TestAnswerOptionViewModel
    {
        public int Id { get; set; }

        public string Content { get; set; } = string.Empty;
    }

    public class SubmitTestViewModel
    {
        [Range(1, int.MaxValue)]
        public int TestId { get; set; }

        public List<QuestionSubmissionViewModel> Answers { get; set; } = new();
    }

    public class QuestionSubmissionViewModel
    {
        [Range(1, int.MaxValue)]
        public int QuestionId { get; set; }

        public int? AnswerId { get; set; }

        public string? TextAnswer { get; set; }
    }

    public class TestResultViewModel
    {
        public int ResultId { get; set; }

        public int TestId { get; set; }

        public string TestTitle { get; set; } = string.Empty;

        public decimal Score { get; set; }

        public int CorrectCount { get; set; }

        public int TotalQuestions { get; set; }

        public int WrongCount => Math.Max(0, TotalQuestions - CorrectCount);

        public DateTime? SubmittedAt { get; set; }

        public bool IsPassed => Score >= 70;

        public List<QuestionResultViewModel> Questions { get; set; } = new();
    }

    public class QuestionResultViewModel
    {
        public int QuestionId { get; set; }

        public int Number { get; set; }

        public string Content { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }

        public string? UserAnswer { get; set; }

        public string? CorrectAnswer { get; set; }
    }

    public class TestHistoryViewModel
    {
        public List<TestHistoryItemViewModel> Items { get; set; } = new();
    }

    public class TestHistoryItemViewModel
    {
        public int ResultId { get; set; }

        public int TestId { get; set; }

        public string TestTitle { get; set; } = string.Empty;

        public string TopicName { get; set; } = string.Empty;

        public decimal Score { get; set; }

        public int CorrectCount { get; set; }

        public int TotalQuestions { get; set; }

        public DateTime? SubmittedAt { get; set; }
    }
}
