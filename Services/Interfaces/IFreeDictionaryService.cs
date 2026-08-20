using SKDJK.Dtos;
using SKDJK.Models.commons;

namespace SKDJK.Services.Interfaces
{
    // Abstraction duy nhất cho việc lấy phiên âm và audio mẫu từ Free Dictionary.
    public interface IFreeDictionaryService
    {
        Task<Result<DictionaryPronunciationDto>> GetPronunciationAsync(string word, CancellationToken cancellationToken = default);
    }
}
