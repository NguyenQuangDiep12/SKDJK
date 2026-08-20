using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SKDJK.Services.Interfaces;

namespace SKDJK.Controllers
{
    // Endpoint nội bộ chỉ proxy ba trường an toàn từ Free Dictionary cho nút loa.
    [Authorize]
    public sealed class DictionaryController : Controller
    {
        private readonly IFreeDictionaryService _dictionaryService;

        public DictionaryController(IFreeDictionaryService dictionaryService)
        {
            _dictionaryService = dictionaryService;
        }

        [HttpGet("dictionary/pronunciation")]
        public async Task<IActionResult> Pronunciation(string word, CancellationToken cancellationToken = default)
        {
            var result = await _dictionaryService.GetPronunciationAsync(word, cancellationToken);
            if (result.IsSuccess)
            {
                return Json(result.Value);
            }

            return result.Error.Code is "Dictionary.NotFound" or "Dictionary.NoAudio" or "Dictionary.Empty"
                ? NotFound(new { message = result.Error.Message })
                : StatusCode(StatusCodes.Status503ServiceUnavailable, new { message = result.Error.Message });
        }
    }
}
