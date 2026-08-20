using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SKDJK.Dtos;
using SKDJK.Services.Interfaces;
using SKDJK.ViewModels;

namespace SKDJK.Controllers
{
    // Điều phối trang học, cập nhật tiến độ và CRUD Lesson/Vocabulary.
    [Authorize]
    public sealed class LessonController : Controller
    {
        private readonly ILessonService _lessonService;

        public LessonController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }

        // Trang Bài học của tôi chỉ lấy tiến độ của tài khoản đang đăng nhập.
        [HttpGet]
        public async Task<IActionResult> MyLessons(CancellationToken cancellationToken = default)
        {
            // UserId luôn lấy từ claim, không nhận từ query string để tránh xem dữ liệu người khác.
            if (!User.TryGetUserId(out int userId))
            {
                return Unauthorized();
            }

            // Service trả DTO chứa duy nhất bài được học gần nhất.
            var result = await _lessonService.GetMyLessonsAsync(userId, cancellationToken);

            // Lỗi tài khoản được trả về HTTP 400 thay vì render dữ liệu không hợp lệ.
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error.Message);
            }

            // Controller chuyển DTO sang ViewModel trước khi đưa dữ liệu cho Razor.
            MyLessonPageViewModel model = new()
            {
                LatestLesson = result.Value.LatestLesson is null
                    ? null
                    : new MyLessonItemViewModel
                {
                    LessonId = result.Value.LatestLesson.LessonId,
                    LessonTitle = result.Value.LatestLesson.LessonTitle,
                    Description = result.Value.LatestLesson.Description,
                    TopicName = result.Value.LatestLesson.TopicName,
                    LanguageName = result.Value.LatestLesson.LanguageName,
                    Status = result.Value.LatestLesson.Status,
                    CompletionPercent = result.Value.LatestLesson.CompletionPercent,
                    LastStudyAt = result.Value.LatestLesson.LastStudyAt
                }
            };

            // Razor chịu trách nhiệm chọn trạng thái trống hoặc danh sách bài học.
            return View(model);
        }

        [HttpGet("lesson/{id:int}")]
        public async Task<IActionResult> Index(int id, CancellationToken cancellationToken = default)
        {
            if (!User.TryGetUserId(out int userId))
            {
                return Unauthorized();
            }

            var result = await _lessonService.GetStudyAsync(id, userId, cancellationToken);
            if (!result.IsSuccess)
            {
                return NotFound(result.Error.Message);
            }

            return View(MapStudy(result.Value));
        }

        [HttpGet("lesson/{id:int}/vocabulary")]
        public async Task<IActionResult> Vocabulary(int id, CancellationToken cancellationToken = default)
        {
            var result = await _lessonService.GetVocabularyAsync(id, cancellationToken);
            return result.IsSuccess
                ? PartialView("Components/_Vocabulary", MapVocabulary(result.Value))
                : NotFound(result.Error.Message);
        }

        [HttpGet("lesson/{id:int}/grammar")]
        public async Task<IActionResult> Grammar(int id, CancellationToken cancellationToken = default)
        {
            var result = await _lessonService.GetGrammarAsync(id, cancellationToken);
            if (!result.IsSuccess)
            {
                return NotFound(result.Error.Message);
            }

            GrammarLearningViewModel model = new() { LessonId = result.Value.LessonId, LessonTitle = result.Value.LessonTitle, Content = result.Value.Content };
            return PartialView("Components/_Grammar", model);
        }

        [HttpGet("lesson/{id:int}/listening")]
        public async Task<IActionResult> Listening(int id, CancellationToken cancellationToken = default)
        {
            var result = await _lessonService.GetListeningAsync(id, cancellationToken);
            return result.IsSuccess
                ? PartialView("Components/_Listening", MapListening(result.Value))
                : NotFound(result.Error.Message);
        }

        [HttpGet("lesson/{id:int}/speaking")]
        public async Task<IActionResult> Speaking(int id, CancellationToken cancellationToken = default)
        {
            var result = await _lessonService.GetVocabularyAsync(id, cancellationToken);
            return result.IsSuccess
                ? PartialView("Components/_Speaking", MapVocabulary(result.Value))
                : NotFound(result.Error.Message);
        }

        [HttpPost("lesson/{id:int}/complete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id, CancellationToken cancellationToken = default)
        {
            if (!User.TryGetUserId(out int userId))
            {
                return Unauthorized();
            }

            var result = await _lessonService.CompleteAsync(id, userId, cancellationToken);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.IsSuccess ? "Đã đánh dấu hoàn thành bài học." : result.Error.Message;
            return RedirectToAction(nameof(Index), new { id });
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> LessonManagementDashboard(int? lessonId, CancellationToken cancellationToken = default)
        {
            var result = await _lessonService.GetAdminPageAsync(lessonId, cancellationToken);
            return result.IsSuccess
                ? View("~/Views/Admin/Lesson/LessonManagementDashboard.cshtml", MapAdminPage(result.Value))
                : BadRequest(result.Error.Message);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken = default)
        {
            var result = await _lessonService.GetLessonFormAsync(null, cancellationToken);
            return View("~/Views/Admin/Lesson/Form.cshtml", MapLessonForm(result.Value));
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
        {
            var result = await _lessonService.GetLessonFormAsync(id, cancellationToken);
            return result.IsSuccess ? View("~/Views/Admin/Lesson/Form.cshtml", MapLessonForm(result.Value)) : NotFound(result.Error.Message);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(AdminLessonFormViewModel model, CancellationToken cancellationToken = default)
        {
            if (ModelState.IsValid)
            {
                // Controller đổi ViewModel của form thành DTO trước khi gọi Service.
                var request = new AdminLessonFormDto
                {
                    Id = model.Id,
                    TopicId = model.TopicId,
                    Title = model.Title,
                    Description = model.Description,
                    Content = model.Content
                };
                var result = await _lessonService.SaveLessonAsync(request, cancellationToken);
                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Đã lưu bài học.";
                    return RedirectToAction(nameof(LessonManagementDashboard), new { lessonId = result.Value });
                }
                ModelState.AddModelError(string.Empty, result.Error.Message);
            }

            var form = await _lessonService.GetLessonFormAsync(model.Id, cancellationToken);
            if (form.IsSuccess)
            {
                model.Topics = form.Value.Topics.Select(item => new AdminTopicOptionViewModel { Id = item.Id, Name = item.Name }).ToList();
            }
            return View("~/Views/Admin/Lesson/Form.cshtml", model);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var result = await _lessonService.DeleteLessonAsync(id, cancellationToken);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.IsSuccess ? "Đã xóa bài học." : result.Error.Message;
            return RedirectToAction(nameof(LessonManagementDashboard));
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> CreateVocabulary(int lessonId, CancellationToken cancellationToken = default)
        {
            var result = await _lessonService.GetVocabularyFormAsync(lessonId, null, cancellationToken);
            return result.IsSuccess ? View("~/Views/Admin/Lesson/VocabularyForm.cshtml", MapVocabularyForm(result.Value)) : NotFound(result.Error.Message);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> EditVocabulary(int id, CancellationToken cancellationToken = default)
        {
            int lessonId = Request.Query.TryGetValue("lessonId", out var values) && int.TryParse(values.FirstOrDefault(), out int parsed) ? parsed : 0;
            var result = await _lessonService.GetVocabularyFormAsync(lessonId, id, cancellationToken);
            return result.IsSuccess ? View("~/Views/Admin/Lesson/VocabularyForm.cshtml", MapVocabularyForm(result.Value)) : NotFound(result.Error.Message);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveVocabulary(AdminVocabularyFormViewModel model, CancellationToken cancellationToken = default)
        {
            if (ModelState.IsValid)
            {
                // Controller đổi ViewModel từ Razor thành DTO thuần dữ liệu.
                var request = new AdminVocabularyFormDto
                {
                    Id = model.Id,
                    LessonId = model.LessonId,
                    Word = model.Word,
                    Meaning = model.Meaning,
                    Pronunciation = model.Pronunciation,
                    Example = model.Example,
                    AudioUrl = model.AudioUrl
                };
                var result = await _lessonService.SaveVocabularyAsync(request, cancellationToken);
                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Đã lưu từ vựng.";
                    return RedirectToAction(nameof(LessonManagementDashboard), new { lessonId = model.LessonId });
                }
                ModelState.AddModelError(string.Empty, result.Error.Message);
            }

            return View("~/Views/Admin/Lesson/VocabularyForm.cshtml", model);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVocabulary(int id, int lessonId, CancellationToken cancellationToken = default)
        {
            var result = await _lessonService.DeleteVocabularyAsync(id, cancellationToken);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.IsSuccess ? "Đã xóa từ vựng." : result.Error.Message;
            return RedirectToAction(nameof(LessonManagementDashboard), new { lessonId });
        }

        private static LessonStudyViewModel MapStudy(LessonStudyDto dto)
        {
            return new LessonStudyViewModel
            {
                LessonId = dto.LessonId,
                LessonTitle = dto.LessonTitle,
                Description = dto.Description,
                TopicName = dto.TopicName,
                CompletionPercent = dto.CompletionPercent,
                LearningStatus = dto.LearningStatus,
                PreviousLessonId = dto.PreviousLessonId,
                NextLessonId = dto.NextLessonId,
                Vocabulary = MapVocabulary(dto.Vocabulary),
                Grammar = new GrammarLearningViewModel { LessonId = dto.Grammar.LessonId, LessonTitle = dto.Grammar.LessonTitle, Content = dto.Grammar.Content },
                Listening = MapListening(dto.Listening)
            };
        }

        private static VocabularyLearningViewModel MapVocabulary(VocabularyLearningDto dto)
        {
            return new VocabularyLearningViewModel
            {
                LessonId = dto.LessonId,
                LessonTitle = dto.LessonTitle,
                Items = dto.Vocabularies.Select(x => new VocabularyItemViewModel
                {
                    VocabularyId = x.VocabularyId,
                    Word = x.Word,
                    Meaning = x.Meaning,
                    Pronunciation = x.Pronunciation,
                    Example = x.Example
                }).ToList()
            };
        }

        private static ListeningLearningViewModel MapListening(ListeningLearningDto dto)
        {
            return new ListeningLearningViewModel
            {
                LessonId = dto.LessonId,
                LessonTitle = dto.LessonTitle,
                Questions = dto.Questions.Select(x => new ListeningQuestionViewModel
                {
                    QuestionId = x.QuestionId,
                    Content = x.Content,
                    AudioUrl = x.AudioUrl,
                    ImageUrl = x.ImageUrl,
                    PartNumber = x.PartNumber,
                    Answers = x.Answers.Select(answer => new TestAnswerOptionViewModel { Id = answer.AnswerId, Content = answer.Content }).ToList()
                }).ToList()
            };
        }

        // Hàm Controller này chỉ đổi DTO quản trị thành ViewModel cho Razor.
        private static AdminLessonPageViewModel MapAdminPage(AdminLessonPageDto dto)
        {
            // Ánh xạ một Lesson được chọn nếu DTO có dữ liệu.
            AdminLessonItemViewModel? selectedLesson = dto.SelectedLesson is null
                ? null
                : new AdminLessonItemViewModel
                {
                    Id = dto.SelectedLesson.Id,
                    Title = dto.SelectedLesson.Title,
                    TopicName = dto.SelectedLesson.TopicName
                };

            // Trả ViewModel hoàn chỉnh cho dashboard hai cột.
            return new AdminLessonPageViewModel
            {
                SelectedLessonId = dto.SelectedLessonId,
                SelectedLesson = selectedLesson,
                Lessons = dto.Lessons.Select(item => new AdminLessonItemViewModel
                {
                    Id = item.Id,
                    Title = item.Title,
                    TopicName = item.TopicName
                }).ToList(),
                Vocabularies = dto.Vocabularies.Select(item => new AdminVocabularyItemViewModel
                {
                    Id = item.Id,
                    LessonId = item.LessonId,
                    Word = item.Word,
                    Meaning = item.Meaning,
                    Pronunciation = item.Pronunciation
                }).ToList()
            };
        }

        // Hàm Controller này đổi DTO form Lesson thành ViewModel có DataAnnotations.
        private static AdminLessonFormViewModel MapLessonForm(AdminLessonFormDto dto)
        {
            return new AdminLessonFormViewModel
            {
                Id = dto.Id,
                TopicId = dto.TopicId,
                Title = dto.Title,
                Description = dto.Description,
                Content = dto.Content,
                Topics = dto.Topics.Select(item => new AdminTopicOptionViewModel { Id = item.Id, Name = item.Name }).ToList()
            };
        }

        // Hàm Controller này đổi DTO form Vocabulary thành ViewModel của Razor.
        private static AdminVocabularyFormViewModel MapVocabularyForm(AdminVocabularyFormDto dto)
        {
            return new AdminVocabularyFormViewModel
            {
                Id = dto.Id,
                LessonId = dto.LessonId,
                Word = dto.Word,
                Meaning = dto.Meaning,
                Pronunciation = dto.Pronunciation,
                Example = dto.Example,
                AudioUrl = dto.AudioUrl
            };
        }
    }
}
