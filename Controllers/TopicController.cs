using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SKDJK.Dtos;
using SKDJK.Services.Interfaces;
using SKDJK.ViewModels;

namespace SKDJK.Controllers
{
    // Điều phối danh sách, chi tiết và phần quản trị chủ đề.
    public sealed class TopicController : Controller
    {
        private readonly ITopicService _topicService;
        private readonly IUploadFile _uploadFile;

        // Nhận Service nghiệp vụ và Service upload; Controller chỉ chuyển file thành URL trước khi tạo DTO.
        public TopicController(ITopicService topicService, IUploadFile uploadFile)
        {
            _topicService = topicService;
            _uploadFile = uploadFile;
        }

        // Hiển thị danh sách chủ đề và giữ các giá trị tìm kiếm, lọc, phân trang trên URL.
        [HttpGet]
        public async Task<IActionResult> Index(
            string? search,
            int? languageId,
            string? level,
            int page = 1,
            int pageSize = 12,
            CancellationToken cancellationToken = default)
        {
            // Service nhận tham số đơn giản và trả DTO cho Controller.
            var result = await _topicService.GetAllAsync(search, languageId, level, page, pageSize, cancellationToken);

            // Dừng action nếu Service không thể tải danh sách.
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error.Message);
            }

            // Controller chuyển DTO danh sách sang ViewModel dành riêng cho Razor.
            TopicListViewModel model = new()
            {
                Search = result.Value.Search,
                LanguageId = result.Value.LanguageId,
                Level = result.Value.Level,
                Page = result.Value.Page,
                PageSize = result.Value.PageSize,
                TotalItems = result.Value.TotalItems,
                Languages = result.Value.Languages.Select(language => new LanguageOptionViewModel
                {
                    Id = language.Id,
                    Name = language.Name
                }).ToList(),
                Levels = result.Value.Levels.ToList(),
                Topics = result.Value.Topics.Select(topic => new TopicCardViewModel
                {
                    Id = topic.Id,
                    Name = topic.Name,
                    Level = topic.Level,
                    LanguageName = topic.LanguageName,
                    Description = topic.Description,
                    ImageUrl = topic.ImageUrl
                }).ToList()
            };

            // Trả ViewModel sang trang Views/Topic/Index.cshtml.
            return View(model);
        }

        // Dùng URL ngắn giống wireframe khi người dùng mở một chủ đề.
        [HttpGet("topics/{id:int}")]
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken = default)
        {
            // Chỉ gửi UserId vào Service khi request đã có claim đăng nhập hợp lệ.
            int? userId = User.TryGetUserId(out int currentUserId) ? currentUserId : null;

            // Service trả DTO chủ đề kèm danh sách bài học và tiến độ của đúng người dùng.
            var result = await _topicService.GetByIdAsync(id, userId, cancellationToken);

            // Chủ đề không tồn tại được trả về HTTP 404.
            if (!result.IsSuccess)
            {
                return NotFound(result.Error.Message);
            }

            // Controller chuyển DTO chi tiết thành ViewModel cho Razor.
            TopicDetailsViewModel model = new()
            {
                Id = result.Value.Id,
                Name = result.Value.Name,
                Level = result.Value.Level,
                LanguageName = result.Value.LanguageName,
                Description = result.Value.Description,
                ImageUrl = result.Value.ImageUrl,
                Lessons = result.Value.Lessons.Select(lesson => new TopicLessonViewModel
                {
                    Id = lesson.Id,
                    Title = lesson.Title,
                    Description = lesson.Description,
                    CompletionPercent = lesson.CompletionPercent
                }).ToList()
            };

            // Trả trang chi tiết đã có dữ liệu bài học.
            return View(model);
        }

        // Dashboard quản trị chỉ cho phép tài khoản ADMIN truy cập.
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> TopicManagementDashboard(
            string? search,
            int? languageId,
            string? level,
            CancellationToken cancellationToken = default)
        {
            // Service tải danh sách quản trị dưới dạng DTO.
            var result = await _topicService.GetAdminAsync(search, languageId, level, cancellationToken);

            // Trả lỗi rõ ràng nếu không tải được dashboard.
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error.Message);
            }

            // Controller ánh xạ DTO sang ViewModel của bảng quản trị.
            AdminTopicListViewModel model = new()
            {
                Search = result.Value.Search,
                LanguageId = result.Value.LanguageId,
                Level = result.Value.Level,
                Languages = result.Value.Languages.Select(language => new LanguageOptionViewModel
                {
                    Id = language.Id,
                    Name = language.Name
                }).ToList(),
                Items = result.Value.Items.Select(topic => new TopicCardViewModel
                {
                    Id = topic.Id,
                    Name = topic.Name,
                    Level = topic.Level,
                    LanguageName = topic.LanguageName,
                    Description = topic.Description,
                    ImageUrl = topic.ImageUrl
                }).ToList()
            };

            // Dashboard nằm trong thư mục Admin nên cần chỉ rõ đường dẫn View.
            return View("~/Views/Admin/Topic/TopicManagementDashboard.cshtml", model);
        }

        // Mở form tạo chủ đề mới cho ADMIN.
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken = default)
        {
            // Service trả DTO trống cùng danh sách ngôn ngữ cho select.
            var result = await _topicService.GetFormAsync(null, cancellationToken);

            // Dừng action nếu danh sách ngôn ngữ không tải được.
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error.Message);
            }

            // Controller đổi DTO form sang ViewModel.
            AdminTopicFormViewModel model = new()
            {
                Id = result.Value.Id,
                LanguageId = result.Value.LanguageId,
                Name = result.Value.Name,
                Level = result.Value.Level,
                Description = result.Value.Description,
                ImageUrl = result.Value.ImageUrl,
                Languages = result.Value.Languages.Select(language => new LanguageOptionViewModel
                {
                    Id = language.Id,
                    Name = language.Name
                }).ToList()
            };

            // Form tạo và sửa dùng chung một file Razor.
            return View("~/Views/Admin/Topic/Form.cshtml", model);
        }

        // Mở form sửa chủ đề hiện có cho ADMIN.
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
        {
            // Service tìm chủ đề và trả DTO form.
            var result = await _topicService.GetFormAsync(id, cancellationToken);

            // Chủ đề không tồn tại được trả về HTTP 404.
            if (!result.IsSuccess)
            {
                return NotFound(result.Error.Message);
            }

            // Controller đổi DTO form sang ViewModel có validation dành cho Razor.
            AdminTopicFormViewModel model = new()
            {
                Id = result.Value.Id,
                LanguageId = result.Value.LanguageId,
                Name = result.Value.Name,
                Level = result.Value.Level,
                Description = result.Value.Description,
                ImageUrl = result.Value.ImageUrl,
                Languages = result.Value.Languages.Select(language => new LanguageOptionViewModel
                {
                    Id = language.Id,
                    Name = language.Name
                }).ToList()
            };

            // Form sửa dùng đúng View chung với form tạo.
            return View("~/Views/Admin/Topic/Form.cshtml", model);
        }

        // POST Create nhận ViewModel và chỉ gọi lệnh CreateAsync.
        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminTopicFormViewModel model, CancellationToken cancellationToken = default)
        {
            // Chỉ gọi Service khi validation của ViewModel hợp lệ.
            if (ModelState.IsValid)
            {
                // Chủ đề mới không nhận URL từ trình duyệt; nếu có file thì tải đúng vào thư mục topics.
                string? imageUrl = null;
                if (model.ImageFile is not null)
                {
                    var uploadResult = await _uploadFile.UploadFileImage(model.ImageFile, "topics", cancellationToken);
                    if (!uploadResult.IsSuccess)
                    {
                        ModelState.AddModelError(nameof(model.ImageFile), uploadResult.Error.Message);
                    }
                    else
                    {
                        imageUrl = uploadResult.Value;
                    }
                }

                // Controller chuyển ViewModel sang DTO Create không chứa Id.
                if (ModelState.IsValid)
                {
                    CreateTopicDto request = new()
                    {
                        LanguageId = model.LanguageId,
                        Name = model.Name,
                        Level = model.Level,
                        Description = model.Description,
                        ImageUrl = imageUrl
                    };

                    // Service chỉ nhận URL Cloudinary trong DTO, không nhận IFormFile của View.
                    var createResult = await _topicService.CreateAsync(request, cancellationToken);

                    // Tạo thành công thì quay lại dashboard quản trị.
                    if (createResult.IsSuccess)
                    {
                        TempData["SuccessMessage"] = "Đã thêm chủ đề.";
                        return RedirectToAction(nameof(TopicManagementDashboard));
                    }

                    // Lỗi nghiệp vụ của Service được hiển thị trong validation summary.
                    ModelState.AddModelError(string.Empty, createResult.Error.Message);
                }
            }

            // Tải lại danh sách ngôn ngữ khi phải render lại form Create.
            var optionsResult = await _topicService.GetFormAsync(null, cancellationToken);

            // Chỉ ánh xạ options khi Service tải thành công.
            if (optionsResult.IsSuccess)
            {
                model.Languages = optionsResult.Value.Languages.Select(language => new LanguageOptionViewModel
                {
                    Id = language.Id,
                    Name = language.Name
                }).ToList();
            }

            // Render lại form Create cùng lỗi validation và dữ liệu vừa nhập.
            return View("~/Views/Admin/Topic/Form.cshtml", model);
        }

        // POST Edit nhận Id riêng từ route và chỉ gọi UpdateAsync.
        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdminTopicFormViewModel model, CancellationToken cancellationToken = default)
        {
            // Id route chỉ được gán vào ViewModel để Razor giữ chế độ sửa khi render lỗi.
            model.Id = id;

            // Chỉ gọi Service khi DataAnnotations của ViewModel hợp lệ.
            if (ModelState.IsValid)
            {
                // Lấy URL hiện tại từ Service để không tin giá trị URL ẩn có thể bị sửa ở trình duyệt.
                var currentResult = await _topicService.GetFormAsync(id, cancellationToken);
                if (!currentResult.IsSuccess)
                {
                    ModelState.AddModelError(string.Empty, currentResult.Error.Message);
                }

                string? imageUrl = currentResult.IsSuccess ? currentResult.Value.ImageUrl : null;

                // Chỉ thay URL cũ khi admin thực sự chọn một file ảnh mới.
                if (ModelState.IsValid && model.ImageFile is not null)
                {
                    var uploadResult = await _uploadFile.UploadFileImage(model.ImageFile, "topics", cancellationToken);
                    if (!uploadResult.IsSuccess)
                    {
                        ModelState.AddModelError(nameof(model.ImageFile), uploadResult.Error.Message);
                    }
                    else
                    {
                        imageUrl = uploadResult.Value;
                    }
                }

                // DTO Update không có Id nên Id URL là nguồn duy nhất.
                if (ModelState.IsValid)
                {
                    UpdateTopicDto request = new()
                    {
                        LanguageId = model.LanguageId,
                        Name = model.Name,
                        Level = model.Level,
                        Description = model.Description,
                        ImageUrl = imageUrl
                    };

                    // Service chỉ cập nhật Topic bằng URL ảnh đã được Controller xác định.
                    var updateResult = await _topicService.UpdateAsync(id, request, cancellationToken);

                    // Cập nhật thành công thì quay lại dashboard.
                    if (updateResult.IsSuccess)
                    {
                        TempData["SuccessMessage"] = "Đã cập nhật chủ đề.";
                        return RedirectToAction(nameof(TopicManagementDashboard));
                    }

                    // Lỗi NotFound, Language hoặc Level được hiển thị trong form.
                    ModelState.AddModelError(string.Empty, updateResult.Error.Message);
                }
            }

            // Lấy lại options Language vì trình duyệt không post toàn bộ danh sách select.
            var optionsResult = await _topicService.GetFormAsync(null, cancellationToken);

            // Giữ dữ liệu người dùng nhập và chỉ bổ sung lại danh sách options.
            if (optionsResult.IsSuccess)
            {
                model.Languages = optionsResult.Value.Languages.Select(language => new LanguageOptionViewModel
                {
                    Id = language.Id,
                    Name = language.Name
                }).ToList();
            }

            // Render lại form Edit với đúng Id route.
            return View("~/Views/Admin/Topic/Form.cshtml", model);
        }

        // Xóa chủ đề bằng POST và anti-forgery để tránh thay đổi dữ liệu qua URL GET.
        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            // Service tự kiểm tra chủ đề có đang chứa bài học hay không.
            var result = await _topicService.DeleteAsync(id, cancellationToken);

            // Chọn thông báo thành công hoặc lỗi để layout hiển thị sau redirect.
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.IsSuccess
                ? "Đã xóa chủ đề."
                : result.Error.Message;

            // Luôn quay lại dashboard để người quản trị thấy trạng thái mới nhất.
            return RedirectToAction(nameof(TopicManagementDashboard));
        }
    }
}
