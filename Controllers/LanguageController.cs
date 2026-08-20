using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SKDJK.Dtos;
using SKDJK.Services.Interfaces;
using SKDJK.ViewModels;

namespace SKDJK.Controllers
{
    // CRUD Language được giới hạn hoàn toàn cho role ADMIN.
    [Authorize(Roles = "ADMIN")]
    public sealed class LanguageController : Controller
    {
        private readonly ILanguageService _languageService;

        public LanguageController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, CancellationToken cancellationToken = default)
        {
            var result = await _languageService.GetAsync(search, cancellationToken);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error.Message);
            }

            // Controller đổi DTO từ Service thành ViewModel dành riêng cho Razor.
            var model = new AdminLanguageListViewModel
            {
                Search = result.Value.Search,
                Items = result.Value.Items.Select(item => new AdminLanguageItemViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Code = item.Code,
                    Description = item.Description,
                    TopicCount = item.TopicCount
                }).ToList()
            };

            return View("~/Views/Admin/Language/Index.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken = default)
        {
            var result = await _languageService.GetFormAsync(null, cancellationToken);
            var model = new AdminLanguageFormViewModel
            {
                Id = result.Value.Id,
                Name = result.Value.Name,
                Code = result.Value.Code,
                Description = result.Value.Description
            };
            return View("~/Views/Admin/Language/Form.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
        {
            var result = await _languageService.GetFormAsync(id, cancellationToken);
            if (!result.IsSuccess)
            {
                return NotFound(result.Error.Message);
            }

            // DTO được chuyển sang ViewModel trước khi gửi tới view sửa ngôn ngữ.
            var model = new AdminLanguageFormViewModel
            {
                Id = result.Value.Id,
                Name = result.Value.Name,
                Code = result.Value.Code,
                Description = result.Value.Description
            };
            return View("~/Views/Admin/Language/Form.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(AdminLanguageFormViewModel model, CancellationToken cancellationToken = default)
        {
            if (ModelState.IsValid)
            {
                // ViewModel chỉ tồn tại đến Controller; Service nhận DTO thuần dữ liệu.
                var request = new AdminLanguageFormDto
                {
                    Id = model.Id,
                    Name = model.Name,
                    Code = model.Code,
                    Description = model.Description
                };
                var result = await _languageService.SaveAsync(request, cancellationToken);
                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Đã lưu ngôn ngữ.";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, result.Error.Message);
            }
            return View("~/Views/Admin/Language/Form.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var result = await _languageService.DeleteAsync(id, cancellationToken);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.IsSuccess ? "Đã xóa ngôn ngữ." : result.Error.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
