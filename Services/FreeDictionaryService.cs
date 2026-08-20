using System.Net;
using System.Text.Json;
using SKDJK.Dtos;
using SKDJK.Models.commons;
using SKDJK.Services.Interfaces;

namespace SKDJK.Services
{
    // Gọi Free Dictionary chỉ để lấy phiên âm và audio mẫu; không ghi dữ liệu phát âm.
    public sealed class FreeDictionaryService : IFreeDictionaryService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FreeDictionaryService> _logger;

        public FreeDictionaryService(HttpClient httpClient, ILogger<FreeDictionaryService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<Result<DictionaryPronunciationDto>> GetPronunciationAsync(string word, CancellationToken cancellationToken = default)
        {
            string normalizedWord = word.Trim();
            if (normalizedWord.Length == 0 || normalizedWord.Length > 100)
            {
                return Result<DictionaryPronunciationDto>
                    .Failure(new Error("Dictionary.InvalidWord", "Từ cần tra không hợp lệ."));
            }

            try
            {
                string encodedWord = Uri.EscapeDataString(normalizedWord);
                using HttpResponseMessage response = await _httpClient
                    .GetAsync($"api/v2/entries/en/{encodedWord}", cancellationToken);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return Result<DictionaryPronunciationDto>
                        .Failure(new Error("Dictionary.NotFound", "Chưa có audio phát âm cho từ này."));
                }

                if (!response.IsSuccessStatusCode)
                {
                    _logger
                        .LogWarning("Free Dictionary trả về HTTP {StatusCode}.", (int)response.StatusCode);
                    return Result<DictionaryPronunciationDto>
                        .Failure(new Error("Dictionary.Unavailable", "Dịch vụ phát âm đang tạm thời không khả dụng."));
                }

                await using Stream stream = await response
                    .Content.ReadAsStreamAsync(cancellationToken);

                using JsonDocument document = await JsonDocument
                    .ParseAsync(stream, cancellationToken: cancellationToken);

                JsonElement entry = document.RootElement.ValueKind == JsonValueKind.Array && document.RootElement.GetArrayLength() > 0
                    ? document.RootElement[0]
                    : default;

                if (entry.ValueKind != JsonValueKind.Object)
                {
                    return Result<DictionaryPronunciationDto>.Failure(new Error("Dictionary.Empty", "Chưa có audio phát âm cho từ này."));
                }

                string? phonetic = entry.TryGetProperty("phonetic", out JsonElement phoneticElement)
                    ? phoneticElement.GetString()
                    : null;
                string? audioUrl = null;

                if (entry.TryGetProperty("phonetics", out JsonElement phonetics) && phonetics.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement item in phonetics.EnumerateArray())
                    {
                        if (string.IsNullOrWhiteSpace(phonetic) && item.TryGetProperty("text", out JsonElement textElement))
                        {
                            phonetic = textElement.GetString();
                        }

                        if (item.TryGetProperty("audio", out JsonElement audioElement) && !string.IsNullOrWhiteSpace(audioElement.GetString()))
                        {
                            audioUrl = audioElement.GetString();
                            break;
                        }
                    }
                }

                if (audioUrl?.StartsWith("//", StringComparison.Ordinal) == true)
                {
                    audioUrl = $"https:{audioUrl}";
                }

                if (string.IsNullOrWhiteSpace(audioUrl))
                {
                    return Result<DictionaryPronunciationDto>.Failure(new Error("Dictionary.NoAudio", "Chưa có audio phát âm cho từ này."));
                }

                DictionaryPronunciationDto dto = new() { Word = normalizedWord, Phonetic = phonetic, AudioUrl = audioUrl };
                return Result<DictionaryPronunciationDto>.Success(dto);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                return Result<DictionaryPronunciationDto>.Failure(new Error("Dictionary.Timeout", "Dịch vụ phát âm phản hồi quá lâu."));
            }
            catch (HttpRequestException exception)
            {
                _logger.LogWarning(exception, "Không thể kết nối Free Dictionary.");
                return Result<DictionaryPronunciationDto>.Failure(new Error("Dictionary.Network", "Không thể kết nối dịch vụ phát âm."));
            }
            catch (JsonException exception)
            {
                _logger.LogWarning(exception, "Free Dictionary trả về dữ liệu không hợp lệ.");
                return Result<DictionaryPronunciationDto>.Failure(new Error("Dictionary.InvalidResponse", "Dịch vụ phát âm trả về dữ liệu không hợp lệ."));
            }
        }
    }
}
