using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SKDJK.Dtos;
using SKDJK.Models.enums;
using SKDJK.Services.Interfaces;
using SKDJK.ViewModels;

namespace SKDJK.Controllers
{
    // Điều phối danh sách, làm bài, kết quả và dashboard quản trị Test.
    [Authorize]
    public sealed class TestController : Controller
    {
        private readonly ITestService _testService;
        private readonly IUploadFile _uploadFile;

        public TestController(ITestService testService, IUploadFile uploadFile)
        {
            _testService = testService;
            _uploadFile = uploadFile;
        }

        // Cung cấp đúng endpoint GET /tests trong đặc tả.
        [HttpGet("tests")]
        public async Task<IActionResult> Index(string? search, TestFormat? format, TestMode? mode, string? level, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            // UserId chỉ lấy từ claim đăng nhập, không nhận từ query string.
            if (!User.TryGetUserId(out int userId))
            {
                return Unauthorized();
            }

            // Service thực hiện filter search/format/mode/level và phân trang.
            var result = await _testService.GetTestsAsync(userId, search, format, mode, level, page, pageSize, cancellationToken);

            // Controller đổi DTO từ Service thành ViewModel trước khi gọi Razor.
            return result.IsSuccess ? View(MapTestList(result.Value)) : BadRequest(result.Error.Message);
        }

        [HttpGet("tests/{id:int}")]
        public async Task<IActionResult> Take(int id, CancellationToken cancellationToken = default)
        {
            var result = await _testService.GetTakeAsync(id, cancellationToken);
            return result.IsSuccess ? View(MapTake(result.Value)) : NotFound(result.Error.Message);
        }

        [HttpPost("tests/{id:int}/submit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(int id, SubmitTestViewModel model, CancellationToken cancellationToken = default)
        {
            if (!User.TryGetUserId(out int userId))
            {
                return Unauthorized();
            }

            // ID trên route là nguồn tin cậy; bỏ lỗi binding cũ của TestId trước khi kiểm tra model.
            model.TestId = id;
            ModelState.Remove(nameof(model.TestId));
            if (!ModelState.IsValid)
            {
                return await RenderTakeWithErrorAsync(id, "Dữ liệu nộp bài không hợp lệ.", cancellationToken);
            }

            // ViewModel từ form được đổi thành DTO trước khi gọi Service chấm bài.
            var request = new SubmitTestDto
            {
                TestId = model.TestId,
                Answers = model.Answers.Select(answer => new QuestionSubmissionDto
                {
                    QuestionId = answer.QuestionId,
                    AnswerId = answer.AnswerId,
                    TextAnswer = answer.TextAnswer
                }).ToList()
            };
            var result = await _testService.SubmitAsync(userId, request, cancellationToken);
            return result.IsSuccess
                ? RedirectToAction(nameof(Result), new { id = result.Value })
                : await RenderTakeWithErrorAsync(id, result.Error.Message, cancellationToken);
        }

        [HttpGet("tests/result/{id:int}")]
        public async Task<IActionResult> Result(int id, CancellationToken cancellationToken = default)
        {
            if (!User.TryGetUserId(out int userId))
            {
                return Unauthorized();
            }

            var result = await _testService.GetResultAsync(userId, id, cancellationToken);
            return result.IsSuccess ? View(MapResult(result.Value)) : NotFound(result.Error.Message);
        }

        [HttpGet]
        public async Task<IActionResult> History(CancellationToken cancellationToken = default)
        {
            if (!User.TryGetUserId(out int userId))
            {
                return Unauthorized();
            }

            var result = await _testService.GetHistoryAsync(userId, cancellationToken);
            return result.IsSuccess ? View(MapHistory(result.Value)) : BadRequest(result.Error.Message);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> TestManagementDashboard(string? search, TestFormat? format, TestMode? mode, CancellationToken cancellationToken = default)
        {
            // Dashboard quản trị lọc theo cả Format và Mode.
            var result = await _testService.GetAdminAsync(search, format, mode, cancellationToken);

            // DTO quản trị được đổi thành ViewModel tại Controller, không truyền Entity ra Razor.
            return result.IsSuccess ? View("~/Views/Admin/Test/TestManagementDashboard.cshtml", MapAdminList(result.Value)) : BadRequest(result.Error.Message);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken = default)
        {
            var result = await _testService.GetTestFormAsync(null, cancellationToken);
            return View("~/Views/Admin/Test/Form.cshtml", MapTestForm(result.Value));
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
        {
            var result = await _testService.GetTestFormAsync(id, cancellationToken);
            return result.IsSuccess ? View("~/Views/Admin/Test/Form.cshtml", MapTestForm(result.Value)) : NotFound(result.Error.Message);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(AdminTestFormViewModel model, CancellationToken cancellationToken = default)
        {
            if (ModelState.IsValid)
            {
                // Controller đổi ViewModel đã hợp lệ thành DTO trước khi lưu Test.
                var request = new AdminTestFormDto
                {
                    Id = model.Id,
                    LessonId = model.LessonId,
                    Title = model.Title,
                    Description = model.Description,
                    DurationMinutes = model.DurationMinutes,
                    Format = model.Format,
                    Mode = model.Mode,
                    IsActive = model.IsActive
                };
                var result = await _testService.SaveTestAsync(request, cancellationToken);
                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Đã lưu bài kiểm tra.";
                    return RedirectToAction(nameof(TestManagementDashboard));
                }
                ModelState.AddModelError(string.Empty, result.Error.Message);
            }

            var form = await _testService.GetTestFormAsync(model.Id, cancellationToken);
            if (form.IsSuccess)
            {
                model.Lessons = form.Value.Lessons.Select(item => new AdminLessonOptionViewModel { Id = item.Id, Title = item.Title }).ToList();
            }
            return View("~/Views/Admin/Test/Form.cshtml", model);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var result = await _testService.DeleteTestAsync(id, cancellationToken);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.IsSuccess ? "Đã xóa bài kiểm tra." : result.Error.Message;
            return RedirectToAction(nameof(TestManagementDashboard));
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> Questions(int testId, CancellationToken cancellationToken = default)
        {
            var result = await _testService.GetQuestionsAsync(testId, cancellationToken);
            return result.IsSuccess ? View("~/Views/Admin/Test/Questions.cshtml", MapQuestionList(result.Value)) : NotFound(result.Error.Message);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> CreateQuestion(int testId, CancellationToken cancellationToken = default)
        {
            var result = await _testService.GetQuestionFormAsync(testId, null, cancellationToken);
            return result.IsSuccess ? View("~/Views/Admin/Test/QuestionForm.cshtml", MapQuestionForm(result.Value)) : NotFound(result.Error.Message);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> EditQuestion(int testId, int id, CancellationToken cancellationToken = default)
        {
            var result = await _testService.GetQuestionFormAsync(testId, id, cancellationToken);
            return result.IsSuccess ? View("~/Views/Admin/Test/QuestionForm.cshtml", MapQuestionForm(result.Value)) : NotFound(result.Error.Message);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveQuestion(AdminQuestionFormViewModel model, CancellationToken cancellationToken = default)
        {
            // Khi sửa, Controller lấy URL hiện có từ Service thay vì tin URL do trình duyệt gửi lên.
            string? imageUrl = null;
            string? audioUrl = null;
            if (model.Id.HasValue)
            {
                var currentResult = await _testService.GetQuestionFormAsync(model.TestId, model.Id, cancellationToken);
                if (!currentResult.IsSuccess)
                {
                    ModelState.AddModelError(string.Empty, currentResult.Error.Message);
                }
                else
                {
                    imageUrl = currentResult.Value.ImageUrl;
                    audioUrl = currentResult.Value.AudioUrl;
                }
            }

            if (ModelState.IsValid)
            {
                // File ảnh của câu hỏi được upload đúng thư mục questions; DTO chỉ nhận URL kết quả.
                if (model.ImageFile is not null)
                {
                    var imageUploadResult = await _uploadFile.UploadFileImage(model.ImageFile, "questions", cancellationToken);
                    if (!imageUploadResult.IsSuccess)
                    {
                        ModelState.AddModelError(nameof(model.ImageFile), imageUploadResult.Error.Message);
                    }
                    else
                    {
                        imageUrl = imageUploadResult.Value;
                    }
                }

                // Cloudinary quản lý audio bằng loại tài nguyên video và lưu trong questionaudio.
                if (ModelState.IsValid && model.AudioFile is not null)
                {
                    var audioUploadResult = await _uploadFile.UploadFileAudio(model.AudioFile, "questionaudio", cancellationToken);
                    if (!audioUploadResult.IsSuccess)
                    {
                        ModelState.AddModelError(nameof(model.AudioFile), audioUploadResult.Error.Message);
                    }
                    else
                    {
                        audioUrl = audioUploadResult.Value;
                    }
                }

                // Controller đổi ViewModel của form câu hỏi thành DTO cho Service.
                if (ModelState.IsValid)
                {
                    var request = new AdminQuestionFormDto
                    {
                        Id = model.Id,
                        TestId = model.TestId,
                        Content = model.Content,
                        QuestionType = model.QuestionType,
                        SectionName = model.SectionName,
                        PartNumber = model.PartNumber,
                        Order = model.Order,
                        GroupCode = model.GroupCode,
                        ContextText = model.ContextText,
                        ImageUrl = imageUrl,
                        AudioUrl = audioUrl,
                        Instruction = model.Instruction,
                        MaxWords = model.MaxWords,
                        Answers = model.Answers.Select(answer => new AdminAnswerInputDto
                        {
                            Content = answer.Content,
                            IsCorrect = answer.IsCorrect
                        }).ToList()
                    };
                    var result = await _testService.SaveQuestionAsync(request, cancellationToken);
                    if (result.IsSuccess)
                    {
                        TempData["SuccessMessage"] = "Đã lưu câu hỏi và đáp án.";
                        return RedirectToAction(nameof(Questions), new { testId = model.TestId });
                    }
                    ModelState.AddModelError(string.Empty, result.Error.Message);
                }
            }

            // Giữ URL hiện tại để form lỗi vẫn hiển thị link xem file đang lưu.
            model.ImageUrl = imageUrl;
            model.AudioUrl = audioUrl;
            while (model.Answers.Count < 4)
            {
                model.Answers.Add(new AdminAnswerInputViewModel());
            }
            return View("~/Views/Admin/Test/QuestionForm.cshtml", model);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQuestion(int id, int testId, CancellationToken cancellationToken = default)
        {
            var result = await _testService.DeleteQuestionAsync(id, cancellationToken);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.IsSuccess ? "Đã xóa câu hỏi." : result.Error.Message;
            return RedirectToAction(nameof(Questions), new { testId });
        }

        // Hàm Controller này đổi DTO danh sách Test thành ViewModel của trang /tests.
        private static TestListViewModel MapTestList(TestListDto dto)
        {
            return new TestListViewModel
            {
                Search = dto.Search,
                Level = dto.Level,
                Format = dto.Format,
                Mode = dto.Mode,
                Page = dto.Page,
                PageSize = dto.PageSize,
                TotalItems = dto.TotalItems,
                Levels = dto.Levels.ToList(),
                Tests = dto.Tests.Select(item => new TestListItemViewModel
                {
                    Id = item.Id,
                    Title = item.Title,
                    LessonTitle = item.LessonTitle,
                    TopicName = item.TopicName,
                    Level = item.Level,
                    QuestionCount = item.QuestionCount,
                    DurationMinutes = item.DurationMinutes,
                    BestScore = item.BestScore,
                    AttemptCount = item.AttemptCount,
                    Format = item.Format,
                    Mode = item.Mode,
                    IsActive = item.IsActive
                }).ToList()
            };
        }

        // Hàm Controller này đổi DTO làm bài an toàn thành ViewModel cho Razor.
        private static TakeTestViewModel MapTake(TakeTestDto dto)
        {
            // Tạo một dictionary để các group dùng lại đúng đối tượng câu hỏi đã ánh xạ.
            Dictionary<int, TestQuestionViewModel> questionsById = dto.Questions.ToDictionary(
                question => question.Id,
                question => MapQuestion(question));

            // Trả ViewModel gồm cả danh sách phẳng và danh sách group theo wireframe.
            return new TakeTestViewModel
            {
                TestId = dto.TestId,
                Title = dto.Title,
                LessonTitle = dto.LessonTitle,
                TopicName = dto.TopicName,
                Level = dto.Level,
                DurationMinutes = dto.DurationMinutes,
                Format = dto.Format,
                Mode = dto.Mode,
                Questions = dto.Questions.Select(question => questionsById[question.Id]).ToList(),
                Groups = dto.Groups.Select(group => new TestQuestionGroupViewModel
                {
                    Key = group.Key,
                    SectionName = group.SectionName,
                    PartNumber = group.PartNumber,
                    GroupCode = group.GroupCode,
                    ContextText = group.ContextText,
                    AudioUrl = group.AudioUrl,
                    ImageUrl = group.ImageUrl,
                    Questions = group.Questions.Select(question => questionsById.TryGetValue(question.Id, out TestQuestionViewModel? mapped)
                        ? mapped
                        : MapQuestion(question)).ToList()
                }).ToList()
            };
        }

        // Hàm Controller này đổi một Question DTO thành Question ViewModel không có đáp án đúng.
        private static TestQuestionViewModel MapQuestion(TestQuestionDto dto)
        {
            return new TestQuestionViewModel
            {
                Id = dto.Id,
                Content = dto.Content,
                QuestionType = dto.QuestionType,
                ImageUrl = dto.ImageUrl,
                AudioUrl = dto.AudioUrl,
                SectionName = dto.SectionName,
                PartNumber = dto.PartNumber,
                Order = dto.Order,
                ContextText = dto.ContextText,
                GroupCode = dto.GroupCode,
                Instruction = dto.Instruction,
                MaxWords = dto.MaxWords,
                Answers = dto.Answers.Select(answer => new TestAnswerOptionViewModel
                {
                    Id = answer.Id,
                    Content = answer.Content
                }).ToList()
            };
        }

        // Hàm Controller này đổi DTO kết quả thành ViewModel có các thuộc tính hiển thị tính toán.
        private static TestResultViewModel MapResult(TestResultDto dto)
        {
            return new TestResultViewModel
            {
                ResultId = dto.ResultId,
                TestId = dto.TestId,
                TestTitle = dto.TestTitle,
                Score = dto.Score,
                CorrectCount = dto.CorrectCount,
                TotalQuestions = dto.TotalQuestions,
                SubmittedAt = dto.SubmittedAt,
                Format = dto.Format,
                Mode = dto.Mode,
                PassingScore = dto.PassingScore,
                Questions = dto.Questions.Select(item => new QuestionResultViewModel
                {
                    QuestionId = item.QuestionId,
                    Number = item.Number,
                    Content = item.Content,
                    SectionName = item.SectionName,
                    IsCorrect = item.IsCorrect,
                    UserAnswer = item.UserAnswer,
                    CorrectAnswer = item.CorrectAnswer
                }).ToList(),
                Sections = dto.Sections.Select(item => new TestSectionResultViewModel
                {
                    SectionName = item.SectionName,
                    CorrectCount = item.CorrectCount,
                    TotalQuestions = item.TotalQuestions
                }).ToList()
            };
        }

        // Hàm Controller này đổi lịch sử DTO thành ViewModel của bảng lịch sử.
        private static TestHistoryViewModel MapHistory(TestHistoryDto dto)
        {
            return new TestHistoryViewModel
            {
                Items = dto.Items.Select(item => new TestHistoryItemViewModel
                {
                    ResultId = item.ResultId,
                    TestId = item.TestId,
                    TestTitle = item.TestTitle,
                    TopicName = item.TopicName,
                    Score = item.Score,
                    CorrectCount = item.CorrectCount,
                    TotalQuestions = item.TotalQuestions,
                    SubmittedAt = item.SubmittedAt,
                    Format = item.Format,
                    Mode = item.Mode
                }).ToList()
            };
        }

        // Hàm Controller này đổi DTO dashboard quản trị thành ViewModel.
        private static AdminTestListViewModel MapAdminList(AdminTestListDto dto)
        {
            return new AdminTestListViewModel
            {
                Search = dto.Search,
                Format = dto.Format,
                Mode = dto.Mode,
                Items = dto.Items.Select(item => new AdminTestItemViewModel
                {
                    Id = item.Id,
                    Title = item.Title,
                    LessonTitle = item.LessonTitle,
                    Format = item.Format,
                    Mode = item.Mode,
                    IsActive = item.IsActive,
                    DurationMinutes = item.DurationMinutes,
                    QuestionCount = item.QuestionCount
                }).ToList()
            };
        }

        // Hàm Controller này đổi DTO form Test thành ViewModel có DataAnnotations.
        private static AdminTestFormViewModel MapTestForm(AdminTestFormDto dto)
        {
            return new AdminTestFormViewModel
            {
                Id = dto.Id,
                LessonId = dto.LessonId,
                Title = dto.Title,
                Description = dto.Description,
                DurationMinutes = dto.DurationMinutes,
                Format = dto.Format,
                Mode = dto.Mode,
                IsActive = dto.IsActive,
                Lessons = dto.Lessons.Select(item => new AdminLessonOptionViewModel { Id = item.Id, Title = item.Title }).ToList()
            };
        }

        // Hàm Controller này đổi DTO danh sách Question thành ViewModel quản trị.
        private static AdminQuestionListViewModel MapQuestionList(AdminQuestionListDto dto)
        {
            return new AdminQuestionListViewModel
            {
                TestId = dto.TestId,
                TestTitle = dto.TestTitle,
                Format = dto.Format,
                Mode = dto.Mode,
                Items = dto.Items.Select(item => new AdminQuestionItemViewModel
                {
                    Id = item.Id,
                    Content = item.Content,
                    QuestionType = item.QuestionType,
                    SectionName = item.SectionName,
                    PartNumber = item.PartNumber,
                    Order = item.Order,
                    AnswerCount = item.AnswerCount
                }).ToList()
            };
        }

        // Hàm Controller này đổi DTO form Question thành ViewModel cho Razor.
        private static AdminQuestionFormViewModel MapQuestionForm(AdminQuestionFormDto dto)
        {
            return new AdminQuestionFormViewModel
            {
                Id = dto.Id,
                TestId = dto.TestId,
                Content = dto.Content,
                QuestionType = dto.QuestionType,
                SectionName = dto.SectionName,
                PartNumber = dto.PartNumber,
                Order = dto.Order,
                GroupCode = dto.GroupCode,
                ContextText = dto.ContextText,
                ImageUrl = dto.ImageUrl,
                AudioUrl = dto.AudioUrl,
                Instruction = dto.Instruction,
                MaxWords = dto.MaxWords,
                Answers = dto.Answers.Select(answer => new AdminAnswerInputViewModel
                {
                    Content = answer.Content,
                    IsCorrect = answer.IsCorrect
                }).ToList()
            };
        }

        private async Task<IActionResult> RenderTakeWithErrorAsync(int testId, string message, CancellationToken cancellationToken)
        {
            var take = await _testService.GetTakeAsync(testId, cancellationToken);
            if (!take.IsSuccess)
            {
                return NotFound(take.Error.Message);
            }

            ModelState.AddModelError(string.Empty, message);
            return View("Take", MapTake(take.Value));
        }
    }
}
