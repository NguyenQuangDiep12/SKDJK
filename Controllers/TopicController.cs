using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SKDJK.Dtos;
using SKDJK.Services.Interfaces;
using SKDJK.ViewModels;

namespace SKDJK.Controllers
{
    public sealed class TopicController : Controller
    {
        private readonly ITopicService _topicService;

        public TopicController(ITopicService topicService)
        {
            _topicService = topicService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, int? languageId, string? level, int page = 1, int pageSize = 12, CancellationToken cancellationToken = default)
        {
            var result = await _topicService.GetTopicsAsync(search, languageId, level, page, pageSize, cancellationToken);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error.Message);
            }

            // Controller ánh xạ DTO của Service thành ViewModel cho trang danh sách.
            var model = new TopicListViewModel
            {
                Search = result.Value.Search,
                LanguageId = result.Value.LanguageId,
                Level = result.Value.Level,
                Page = result.Value.Page,
                PageSize = result.Value.PageSize,
                TotalItems = result.Value.TotalItems,
                Levels = result.Value.Levels.ToList(),
                Languages = result.Value.Languages.Select(item => new LanguageOptionViewModel { Id = item.Id, Name = item.Name }).ToList(),
                Topics = result.Value.Topics.Select(item => new TopicCardViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Level = item.Level,
                    LanguageName = item.LanguageName,
                    Description = item.Description,
                    ImageUrl = item.ImageUrl
                }).ToList()
            };

            return View(model);
        }

        [HttpGet("topics/{id:int}")]
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken = default)
        {
            int? userId = User.TryGetUserId(out int currentUserId) ? currentUserId : null;
            var result = await _topicService.GetDetailsAsync(id, userId, cancellationToken);
            if (!result.IsSuccess)
            {
                return NotFound(result.Error.Message);
            }

            // DTO chi tiết được đổi thành ViewModel trước khi đưa cho Razor.
            var model = new TopicDetailsViewModel
            {
                Id = result.Value.Id,
                Name = result.Value.Name,
                Level = result.Value.Level,
                LanguageName = result.Value.LanguageName,
                Description = result.Value.Description,
                ImageUrl = result.Value.ImageUrl,
                Lessons = result.Value.Lessons.Select(item => new TopicLessonViewModel
                {
                    Id = item.Id,
                    Title = item.Title,
                    Description = item.Description,
                    CompletionPercent = item.CompletionPercent
                }).ToList()
            };

            return View(model);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> TopicManagementDashboard(string? search, int? languageId, string? level, CancellationToken cancellationToken = default)
        {
            var result = await _topicService.GetAdminAsync(search, languageId, level, cancellationToken);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error.Message);
            }

            // Controller tạo ViewModel của dashboard từ DTO quản trị.
            var model = new AdminTopicListViewModel
            {
                Search = result.Value.Search,
                LanguageId = result.Value.LanguageId,
                Level = result.Value.Level,
                Languages = result.Value.Languages.Select(item => new LanguageOptionViewModel { Id = item.Id, Name = item.Name }).ToList(),
                Items = result.Value.Items.Select(item => new TopicCardViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Level = item.Level,
                    LanguageName = item.LanguageName,
                    Description = item.Description,
                    ImageUrl = item.ImageUrl
                }).ToList()
            };

            return View("~/Views/Admin/Topic/TopicManagementDashboard.cshtml", model);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken = default)
        {
            var result = await _topicService.GetFormAsync(null, cancellationToken);
            var model = new AdminTopicFormViewModel
            {
                Id = result.Value.Id,
                LanguageId = result.Value.LanguageId,
                Name = result.Value.Name,
                Level = result.Value.Level,
                Description = result.Value.Description,
                ImageUrl = result.Value.ImageUrl,
                Languages = result.Value.Languages.Select(item => new LanguageOptionViewModel { Id = item.Id, Name = item.Name }).ToList()
            };
            return View("~/Views/Admin/Topic/Form.cshtml", model);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
        {
            var result = await _topicService.GetFormAsync(id, cancellationToken);
            if (!result.IsSuccess)
            {
                return NotFound(result.Error.Message);
            }

            // DTO form sửa được chuyển thành ViewModel có validation dành cho giao diện.
            var model = new AdminTopicFormViewModel
            {
                Id = result.Value.Id,
                LanguageId = result.Value.LanguageId,
                Name = result.Value.Name,
                Level = result.Value.Level,
                Description = result.Value.Description,
                ImageUrl = result.Value.ImageUrl,
                Languages = result.Value.Languages.Select(item => new LanguageOptionViewModel { Id = item.Id, Name = item.Name }).ToList()
            };
            return View("~/Views/Admin/Topic/Form.cshtml", model);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(AdminTopicFormViewModel model, CancellationToken cancellationToken = default)
        {
            if (ModelState.IsValid)
            {
                // ViewModel đã hợp lệ được chuyển thành DTO trước khi đi vào Service.
                var request = new AdminTopicFormDto
                {
                    Id = model.Id,
                    LanguageId = model.LanguageId,
                    Name = model.Name,
                    Level = model.Level,
                    Description = model.Description,
                    ImageUrl = model.ImageUrl
                };
                var saveResult = await _topicService.SaveAsync(request, cancellationToken);
                if (saveResult.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Đã lưu chủ đề.";
                    return RedirectToAction(nameof(TopicManagementDashboard));
                }

                ModelState.AddModelError(string.Empty, saveResult.Error.Message);
            }

            var formResult = await _topicService.GetFormAsync(model.Id, cancellationToken);
            if (formResult.IsSuccess)
            {
                model.Languages = formResult.Value.Languages.Select(item => new LanguageOptionViewModel { Id = item.Id, Name = item.Name }).ToList();
            }
            return View("~/Views/Admin/Topic/Form.cshtml", model);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var result = await _topicService.DeleteAsync(id, cancellationToken);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.IsSuccess ? "Đã xóa chủ đề." : result.Error.Message;
            return RedirectToAction(nameof(TopicManagementDashboard));
        }
    }
}
