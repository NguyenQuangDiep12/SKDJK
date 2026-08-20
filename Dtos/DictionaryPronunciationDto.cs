namespace SKDJK.Dtos
{
    // Chỉ giữ ba trường UI cần từ Free Dictionary thay vì lưu toàn bộ response.
    public sealed class DictionaryPronunciationDto
    {
        public string Word { get; init; } = string.Empty;
        public string? Phonetic { get; init; }
        public string? AudioUrl { get; init; }
    }
}
