namespace SKDJK.ViewModels
{
    public class TopicListViewModel
    {
        public string? Search { get; set; }

        public int? LanguageId { get; set; }

        public string? Level { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 12;

        public int TotalItems { get; set; }

        public int TotalPages =>
            PageSize <= 0 ? 1 : (int)Math.Ceiling((double)TotalItems / PageSize);

        public List<LanguageOptionViewModel> Languages { get; set; } = new();

        public List<string> Levels { get; set; } = new();

        public List<TopicCardViewModel> Topics { get; set; } = new();
    }

    public class TopicCardViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Level { get; set; } = string.Empty;

        public string LanguageName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }
    }

    public class LanguageOptionViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }

    public class TopicDetailsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Level { get; set; } = string.Empty;

        public string LanguageName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public List<TopicLessonViewModel> Lessons { get; set; } = new();
    }

    public class TopicLessonViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal CompletionPercent { get; set; }

        public bool IsCompleted => CompletionPercent >= 100;
    }
}
