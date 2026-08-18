using Microsoft.AspNetCore.Mvc;
using SKDJK.Dtos;
using SKDJK.Services.Interfaces;
using SKDJK.ViewModels;

namespace SKDJK.Controllers
{
    public class LessonController : Controller
    {
        private readonly ILessonService _lessonService;
        public LessonController(ILessonService lessonService) 
        {
            _lessonService = lessonService;
        }
        [HttpGet("Lesson/{id:int}")]
        public IActionResult Index(int id)
        {
            ViewBag.LessonId = id;
            return View();
        }   
        [HttpGet("lesson/{id:int}/vocabulary")]
        public async Task<IActionResult> Vocabulary(int id, CancellationToken ct)
        {
            var result = await _lessonService.GetVocabularyAsync(id, ct);

            if (!result.IsSuccess)
            {
                return NotFound();
            }

            var dto = result.Value;

            var viewModel = new VocabularyLearningViewModel
            {
                LessonId = dto.LessonId,
                LessonTitle = dto.LessonTitle,
                Items = dto.Vocabularies.Select(v => new VocabularyItemViewModel
                {
                    AudioUrl = v.AudioUrl,
                    Example = v.Example,
                    Meaning = v.Meaning,
                    Pronunciation = v.Pronunciation,
                    VocabularyId = v.VocabularyId,
                    Word = v.Word,
                }).ToList()
            };

            return PartialView("_Vocabulary", viewModel);
        }

        [HttpGet("lesson/{id:int}/grammar")]
        public async Task<IActionResult> Grammar(int id, CancellationToken ct)
        {
            return PartialView("_Grammar");
        }

        [HttpGet("lesson/{id:int}/listening")]
        public async Task<IActionResult> Listening(int id, CancellationToken ct)
        {
            return PartialView("_Listening");
        }
        [HttpGet("lesson/{id:int}/speaking")]
        public async Task<IActionResult> Speaking(int id, CancellationToken ct)
        {
            return PartialView("_Speaking");
        }
    }
}
