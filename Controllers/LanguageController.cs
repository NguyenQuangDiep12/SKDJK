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
            // Trang danh sách gọi đúng CRUD read-all của LanguageService.
            var result = await _languageService.GetAllAsync(search, cancellationToken);

            // Dừng action nếu Service không tải được dữ liệu.
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

        // GET Create chỉ cần ViewModel trống và không gọi lệnh ghi dữ liệu.
        [HttpGet]
        public IActionResult Create()
        {
            // ViewModel thuộc biên Razor và không được truyền xuống Service.
            return View("~/Views/Admin/Language/Form.cshtml", new AdminLanguageFormViewModel());
        }

        // GET Edit đọc một Language bằng CRUD GetByIdAsync.
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
        {
            // Id được lấy từ route rồi truyền riêng vào Service.
            var result = await _languageService.GetByIdAsync(id, cancellationToken);

            // Không tìm thấy Language thì trả HTTP 404.
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

        // POST Create chỉ gọi CreateAsync và không chứa nhánh Update.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminLanguageFormViewModel model, CancellationToken cancellationToken = default)
        {
            // Trả lại đúng form Create khi DataAnnotations chưa hợp lệ.
            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/Language/Form.cshtml", model);
            }

            // Controller đổi ViewModel thành DTO dành riêng cho lệnh Create.
            CreateLanguageDto request = new()
            {
                Name = model.Name,
                Code = model.Code,
                Description = model.Description
            };

            // Service chỉ thực hiện CREATE và trả Id mới.
            var result = await _languageService.CreateAsync(request, cancellationToken);

            // Hiển thị lỗi duplicate hoặc validation của Service ngay trên form.
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Error.Message);
                return View("~/Views/Admin/Language/Form.cshtml", model);
            }

            // Thông báo thành công được layout hiển thị sau redirect.
            TempData["SuccessMessage"] = "Đã thêm ngôn ngữ.";

            // Quay lại danh sách sau khi tạo thành công.
            return RedirectToAction(nameof(Index));
        }

        // POST Edit lấy Id riêng từ route và chỉ gọi UpdateAsync.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdminLanguageFormViewModel model, CancellationToken cancellationToken = default)
        {
            // Gán Id route vào ViewModel chỉ để View nhận biết đang ở chế độ sửa khi render lỗi.
            model.Id = id;

            // Không gọi Service nếu ViewModel chưa hợp lệ.
            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/Language/Form.cshtml", model);
            }

            // DTO Update không chứa Id nên không thể xung đột với Id trên URL.
            UpdateLanguageDto request = new()
            {
                Name = model.Name,
                Code = model.Code,
                Description = model.Description
            };

            // Service chỉ cập nhật Language có Id được truyền riêng.
            var result = await _languageService.UpdateAsync(id, request, cancellationToken);

            // Lỗi NotFound hoặc duplicate được đưa vào validation summary.
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Error.Message);
                return View("~/Views/Admin/Language/Form.cshtml", model);
            }

            // Ghi thông báo riêng cho thao tác cập nhật.
            TempData["SuccessMessage"] = "Đã cập nhật ngôn ngữ.";

            // Quay về bảng quản trị Language.
            return RedirectToAction(nameof(Index));
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
