using Microsoft.EntityFrameworkCore;
using SKDJK.Data;
using SKDJK.Dtos;
using SKDJK.Models;
using SKDJK.Models.commons;
using SKDJK.Models.enums;
using SKDJK.Services.Interfaces;

namespace SKDJK.Services
{
    // Chứa toàn bộ nghiệp vụ Test để Controller chỉ điều phối request và response.
    public sealed class TestService : ITestService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly decimal _passingScore;

        public TestService(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _passingScore = configuration.GetValue<decimal?>("Testing:PassingScore") ?? 70m;
        }

        public async Task<Result<TestListDto>> GetTestsAsync(
            int userId, 
            string? search, 
            TestFormat? format, 
            TestMode? mode, 
            string? level, 
            int page = 1, 
            int pageSize = 10, 
            CancellationToken cancellationToken = default)
        {
            // Không cho page nhỏ hơn 1 để phép Skip không nhận số âm.
            page = Math.Max(1, page);

            // Giới hạn pageSize để một request không tải quá nhiều bản ghi.
            pageSize = Math.Clamp(pageSize, 1, 50);

            // Người học chỉ nhìn thấy các đề đang hoạt động.
            IQueryable<Test> query = _dbContext.Tests.AsNoTracking().Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
            {
                string value = search.Trim();
                query = query.Where(x => x.Title.Contains(value));
            }

            if (format.HasValue)
            {
                query = query.Where(x => x.Format == format.Value);
            }

            // Lọc riêng Practice/FullMock khi người dùng chọn Mode.
            if (mode.HasValue)
            {
                query = query.Where(x => x.Mode == mode.Value);
            }

            if (!string.IsNullOrWhiteSpace(level))
            {
                string value = level.Trim();
                query = query.Where(x => x.Lesson.Topic.Level == value);
            }

            int totalItems = await query.CountAsync(cancellationToken);
            List<TestListItemDto> tests = await query
                .OrderBy(x => x.Format)
                .ThenBy(x => x.Title)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new TestListItemDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    LessonTitle = x.Lesson.Title,
                    TopicName = x.Lesson.Topic.Name,
                    Level = x.Lesson.Topic.Level,
                    Format = x.Format,
                    Mode = x.Mode,
                    IsActive = x.IsActive,
                    QuestionCount = x.Questions.Count,
                    DurationMinutes = x.DurationMinutes,
                    AttemptCount = x.TestResults.Count(result => result.UserId == userId),
                    BestScore = x.TestResults.Where(result => result.UserId == userId).Select(result => (decimal?)result.Score).Max()
                })
                .ToListAsync(cancellationToken);

            TestListDto dto = new()
            {
                Search = search,
                Format = format,
                Mode = mode,
                Level = level,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                Tests = tests,
                Levels = await _dbContext.Topics.AsNoTracking().Select(x => x.Level).Distinct().OrderBy(x => x).ToListAsync(cancellationToken)
            };

            return Result<TestListDto>.Success(dto);
        }

        public async Task<Result<TakeTestDto>> GetTakeAsync(int testId, CancellationToken cancellationToken = default)
        {
            TakeTestDto? dto = await _dbContext.Tests
                .AsNoTracking()
                // Tách truy vấn Questions/Answers để tránh tích chéo collection lớn.
                .AsSplitQuery()
                .Where(x => x.Id == testId && x.IsActive)
                .Select(x => new TakeTestDto
                {
                    TestId = x.Id,
                    Title = x.Title,
                    LessonTitle = x.Lesson.Title,
                    TopicName = x.Lesson.Topic.Name,
                    Level = x.Lesson.Topic.Level,
                    DurationMinutes = x.DurationMinutes,
                    Format = x.Format,
                    Mode = x.Mode,
                    Questions = x.Questions
                        .OrderBy(question => question.Order)
                        .ThenBy(question => question.Id)
                        .Select(question => new TestQuestionDto
                        {
                            Id = question.Id,
                            Content = question.Content,
                            QuestionType = question.QuestionType,
                            SectionName = question.SectionName,
                            PartNumber = question.PartNumber,
                            Order = question.Order,
                            ContextText = question.ContextText,
                            GroupCode = question.GroupCode,
                            Instruction = question.Instruction,
                            MaxWords = question.MaxWords,
                            AudioUrl = question.AudioUrl,
                            ImageUrl = question.ImageUrl,
                            Answers = question.Answers
                                .OrderBy(answer => answer.Id)
                                .Select(answer => new TestAnswerOptionDto { Id = answer.Id, Content = answer.Content })
                                .ToList()
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            // Trả lỗi nếu ID không tồn tại hoặc đề đang bị tắt.
            if (dto is null)
            {
                return Result<TakeTestDto>.Failure(new Error("Test.NotFound", "Không tìm thấy bài kiểm tra đang hoạt động."));
            }

            // Gom câu trực tiếp trong GetTakeAsync để không dùng BuildQuestionGroups private.
            Dictionary<string, TestQuestionGroupDto> groupsByKey = new(StringComparer.OrdinalIgnoreCase);
            foreach (TestQuestionDto question in dto.Questions.OrderBy(x => x.Order).ThenBy(x => x.Id))
            {
                string groupValue = string.IsNullOrWhiteSpace(question.GroupCode) ? $"QUESTION-{question.Id}" : question.GroupCode;
                string key = $"{question.SectionName}|{question.PartNumber}|{groupValue}";

                if (!groupsByKey.TryGetValue(key, out TestQuestionGroupDto? group))
                {
                    group = new TestQuestionGroupDto
                    {
                        Key = key,
                        SectionName = question.SectionName,
                        PartNumber = question.PartNumber,
                        GroupCode = question.GroupCode,
                        ContextText = question.ContextText,
                        AudioUrl = question.AudioUrl,
                        ImageUrl = question.ImageUrl
                    };

                    groupsByKey.Add(key, group);
                    dto.Groups.Add(group);
                }

                group.Questions.Add(question);
            }

            // DTO chỉ nhận option text, tuyệt đối không nhận Answer.IsCorrect.
            return Result<TakeTestDto>.Success(dto);
        }

        public async Task<Result<int>> SubmitAsync(int userId, SubmitTestDto request, CancellationToken cancellationToken = default)
        {
            // Xác nhận UserId lấy từ claim vẫn còn tồn tại trong database.
            bool userExists = await _dbContext.Users.AsNoTracking().AnyAsync(x => x.Id == userId, cancellationToken);

            // Dừng ngay nếu tài khoản đã bị xóa hoặc claim không hợp lệ.
            if (!userExists)
            {
                return Result<int>.Failure(new Error("User.NotFound", "Không tìm thấy người học."));
            }

            // Server tự tải câu hỏi và IsCorrect; không tin dữ liệu chấm từ browser.
            Test? test = await _dbContext.Tests
                .AsNoTracking()
                // Tách Test, Questions và Answers thành các SELECT rõ ràng khi chấm.
                .AsSplitQuery()
                .Include(x => x.Questions)
                .ThenInclude(x => x.Answers)
                .FirstOrDefaultAsync(x => x.Id == request.TestId && x.IsActive, cancellationToken);

            // Chỉ đề đang hoạt động mới được nộp.
            if (test is null)
            {
                return Result<int>.Failure(new Error("Test.NotFound", "Không tìm thấy bài kiểm tra đang hoạt động."));
            }

            // Một QuestionId xuất hiện hai lần có thể gây ghi đè nên phải từ chối.
            if (request.Answers.GroupBy(x => x.QuestionId).Any(group => group.Count() > 1))
            {
                return Result<int>.Failure(new Error("Test.DuplicateAnswer", "Mỗi câu hỏi chỉ được gửi một câu trả lời."));
            }

            // Tạo tập ID hợp lệ trực tiếp từ đề vừa tải.
            HashSet<int> validQuestionIds = test.Questions.Select(x => x.Id).ToHashSet();

            // Không cho client chèn câu hỏi của đề khác vào request.
            if (request.Answers.Any(x => !validQuestionIds.Contains(x.QuestionId)))
            {
                return Result<int>.Failure(new Error("Test.InvalidQuestion", "Có câu hỏi không thuộc bài kiểm tra."));
            }

            // Dictionary giúp tìm câu trả lời theo QuestionId trong O(1).
            Dictionary<int, QuestionSubmissionDto> submittedByQuestion = request.Answers.ToDictionary(x => x.QuestionId);

            // Tạo kết quả nhưng chưa lưu cho đến khi tất cả câu đã được kiểm tra.
            TestResult result = new()
            {
                UserId = userId,
                TestId = test.Id,
                TotalQuestions = test.Questions.Count,
                SubmittedAt = DateTime.UtcNow
            };

            // Biến này chỉ được tăng từ kết quả chấm trên server.
            int correctCount = 0;

            // Chấm lần lượt theo Order để lịch sử giữ đúng thứ tự hiển thị.
            foreach (Question question in test.Questions.OrderBy(x => x.Order).ThenBy(x => x.Id))
            {
                // Câu chưa trả lời sẽ cho submitted bằng null và được tính là sai.
                submittedByQuestion.TryGetValue(question.Id, out QuestionSubmissionDto? submitted);

                // Chỉ hai loại completion mới nhận text.
                bool isTextQuestion = question.QuestionType is QuestionType.FillBlank or QuestionType.ListeningFill;

                // AnswerId chỉ dùng cho câu lựa chọn.
                int? storedAnswerId = null;

                // TextAnswer chỉ dùng cho câu nhập text.
                string? storedText = null;

                // Mặc định câu chưa trả lời hoặc không khớp là sai.
                bool isCorrect = false;

                // Nhánh này chấm FillBlank và ListeningFill.
                if (isTextQuestion)
                {
                    // Giữ text đã trim để hiển thị lịch sử nhưng không gán AnswerId.
                    storedText = submitted?.TextAnswer?.Trim() ?? string.Empty;

                    // Chặn dữ liệu quá dài trước khi ghi DB.
                    if (storedText.Length > 200)
                    {
                        return Result<int>.Failure(new Error("Test.AnswerTooLong", "Câu trả lời không được vượt quá 200 ký tự."));
                    }

                    // Chuẩn hóa khoảng trắng và chữ hoa/thường đúng đặc tả.
                    string normalized = string.Join(" ", storedText.Trim().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)).ToLowerInvariant();

                    // Vượt MaxWords làm câu sai, kể cả nội dung còn lại khớp đáp án.
                    int wordCount = storedText.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length;
                    bool exceedsWordLimit = question.MaxWords.HasValue && wordCount > question.MaxWords.Value;

                    // Một câu completion có thể chấp nhận nhiều Answer.IsCorrect khác nhau.
                    Answer? matched = question.Answers.FirstOrDefault(answer => answer.IsCorrect
                        && string.Join(" ", answer.Content.Trim().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)).ToLowerInvariant() == normalized);

                    // Câu chỉ đúng khi có nội dung, không vượt giới hạn và khớp một đáp án cho phép.
                    isCorrect = normalized.Length > 0 && !exceedsWordLimit && matched is not null;
                }
                // Nhánh này chấm các loại lựa chọn.
                else if (submitted?.AnswerId is int answerId)
                {
                    // Chỉ tìm đáp án trong collection của chính Question hiện tại.
                    Answer? selected = question.Answers.FirstOrDefault(answer => answer.Id == answerId);

                    // ID có thật nhưng thuộc câu khác vẫn bị từ chối.
                    if (selected is null)
                    {
                        return Result<int>.Failure(new Error("Test.InvalidAnswer", "Có đáp án không thuộc câu hỏi tương ứng."));
                    }

                    // Lưu AnswerId để xem lại lịch sử.
                    storedAnswerId = selected.Id;

                    // Câu lựa chọn không ghi trùng nội dung vào TextAnswer.
                    storedText = null;

                    // IsCorrect chỉ được đọc từ Answer trong database.
                    isCorrect = selected.IsCorrect;
                }

                // Tăng tổng đúng sau khi server chấm xong câu hiện tại.
                if (isCorrect)
                {
                    correctCount++;
                }

                // Lưu chi tiết từng câu để Result có thể xem lại sau khi submit.
                result.UserAnswers.Add(new UserAnswer
                {
                    QuestionId = question.Id,
                    AnswerId = storedAnswerId,
                    TextAnswer = storedText,
                    IsCorrect = isCorrect
                });
            }

            // Lưu số câu đúng đã tính trên server.
            result.CorrectCount = correctCount;

            // Điểm SKDJK là phần trăm nội bộ, không phải scaled score/band score chính thức.
            result.Score = test.Questions.Count == 0 ? 0 : Math.Round(correctCount * 100m / test.Questions.Count, 2);

            // Transaction bảo đảm TestResult và toàn bộ UserAnswer cùng thành công hoặc cùng rollback.
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            // Đưa aggregate TestResult vào change tracker; UserAnswers sẽ được cascade insert.
            _dbContext.TestResults.Add(result);

            // Ghi kết quả và chi tiết đáp án trong cùng transaction.
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Chỉ commit sau khi SaveChanges hoàn tất không lỗi.
            await transaction.CommitAsync(cancellationToken);

            // Trả ID kết quả để Controller redirect đến trang Result.
            return Result<int>.Success(result.Id);
        }

        public async Task<Result<TestResultDto>> GetResultAsync(int userId, int resultId, CancellationToken cancellationToken = default)
        {
            TestResult? result = await _dbContext.TestResults
                .AsNoTracking()
                // Tách Questions/Answers và UserAnswers để không nhân bản bản ghi trong JOIN.
                .AsSplitQuery()
                .Include(x => x.Test)
                .ThenInclude(x => x.Questions)
                .ThenInclude(x => x.Answers)
                .Include(x => x.UserAnswers)
                .ThenInclude(x => x.Answer)
                .FirstOrDefaultAsync(x => x.Id == resultId && x.UserId == userId, cancellationToken);

            if (result is null)
            {
                return Result<TestResultDto>.Failure(new Error("TestResult.NotFound", "Không tìm thấy kết quả bài kiểm tra."));
            }

            Dictionary<int, UserAnswer> userAnswers = result.UserAnswers.ToDictionary(x => x.QuestionId);
            List<QuestionResultDto> questionResults = [];
            int number = 1;

            foreach (Question question in result.Test.Questions.OrderBy(x => x.Order).ThenBy(x => x.Id))
            {
                userAnswers.TryGetValue(question.Id, out UserAnswer? userAnswer);
                questionResults.Add(new QuestionResultDto
                {
                    QuestionId = question.Id,
                    Number = number++,
                    Content = question.Content,
                    SectionName = question.SectionName,
                    IsCorrect = userAnswer?.IsCorrect == true,
                    UserAnswer = userAnswer?.Answer?.Content ?? userAnswer?.TextAnswer,
                    CorrectAnswer = string.Join(" / ", question.Answers.Where(x => x.IsCorrect).Select(x => x.Content))
                });
            }

            TestResultDto dto = new()
            {
                ResultId = result.Id,
                TestId = result.TestId,
                TestTitle = result.Test.Title,
                Format = result.Test.Format,
                Mode = result.Test.Mode,
                Score = result.Score,
                CorrectCount = result.CorrectCount,
                TotalQuestions = result.TotalQuestions > 0 ? result.TotalQuestions : result.Test.Questions.Count,
                SubmittedAt = result.SubmittedAt,
                PassingScore = _passingScore,
                Questions = questionResults,
                Sections = questionResults
                    .GroupBy(x => x.SectionName)
                    .Select(group => new TestSectionResultDto
                    {
                        SectionName = group.Key,
                        CorrectCount = group.Count(x => x.IsCorrect),
                        TotalQuestions = group.Count()
                    })
                    .OrderBy(x => x.SectionName == "Listening" ? 0 : 1)
                    .ToList()
            };

            return Result<TestResultDto>.Success(dto);
        }

        public async Task<Result<TestHistoryDto>> GetHistoryAsync(int userId, CancellationToken cancellationToken = default)
        {
            List<TestHistoryItemDto> items = await _dbContext.TestResults
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.SubmittedAt)
                .Select(x => new TestHistoryItemDto
                {
                    ResultId = x.Id,
                    TestId = x.TestId,
                    TestTitle = x.Test.Title,
                    TopicName = x.Test.Lesson.Topic.Name,
                    Format = x.Test.Format,
                    Mode = x.Test.Mode,
                    Score = x.Score,
                    CorrectCount = x.CorrectCount,
                    TotalQuestions = x.TotalQuestions > 0 ? x.TotalQuestions : x.Test.Questions.Count,
                    SubmittedAt = x.SubmittedAt
                })
                .ToListAsync(cancellationToken);

            return Result<TestHistoryDto>.Success(new TestHistoryDto { Items = items });
        }

        public async Task<Result<AdminTestListDto>> GetAdminAsync(string? search, TestFormat? format, TestMode? mode, CancellationToken cancellationToken = default)
        {
            IQueryable<Test> query = _dbContext.Tests.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(search))
            {
                string value = search.Trim();
                query = query.Where(x => x.Title.Contains(value));
            }

            if (format.HasValue)
            {
                query = query.Where(x => x.Format == format.Value);
            }

            if (mode.HasValue)
            {
                query = query.Where(x => x.Mode == mode.Value);
            }

            AdminTestListDto dto = new()
            {
                Search = search,
                Format = format,
                Mode = mode,
                Items = await query
                    .OrderBy(x => x.Title)
                    .Select(x => new AdminTestItemDto
                    {
                        Id = x.Id,
                        Title = x.Title,
                        LessonTitle = x.Lesson.Title,
                        Format = x.Format,
                        Mode = x.Mode,
                        IsActive = x.IsActive,
                        DurationMinutes = x.DurationMinutes,
                        QuestionCount = x.Questions.Count
                    })
                    .ToListAsync(cancellationToken)
            };

            return Result<AdminTestListDto>.Success(dto);
        }

        public async Task<Result<AdminTestFormDto>> GetTestFormAsync(int? id, CancellationToken cancellationToken = default)
        {
            AdminTestFormDto dto;
            if (id.HasValue)
            {
                dto = await _dbContext.Tests
                    .AsNoTracking()
                    .Where(x => x.Id == id.Value)
                    .Select(x => new AdminTestFormDto
                    {
                        Id = x.Id,
                        LessonId = x.LessonId,
                        Title = x.Title,
                        Description = x.Description,
                        DurationMinutes = x.DurationMinutes,
                        Format = x.Format,
                        Mode = x.Mode,
                        IsActive = x.IsActive
                    })
                    .FirstOrDefaultAsync(cancellationToken) ?? null!;

                if (dto is null)
                {
                    return Result<AdminTestFormDto>.Failure(new Error("Test.NotFound", "Không tìm thấy bài kiểm tra."));
                }
            }
            else
            {
                dto = new AdminTestFormDto();
            }

            // Options Lesson được tải trực tiếp trong hàm form, không gọi helper private.
            dto.Lessons = await _dbContext.Lessons
                .AsNoTracking()
                .OrderBy(x => x.Topic.Name)
                .ThenBy(x => x.Title)
                .Select(x => new AdminLessonOptionDto { Id = x.Id, Title = $"{x.Topic.Name} - {x.Title}" })
                .ToListAsync(cancellationToken);

            return Result<AdminTestFormDto>.Success(dto);
        }

        public async Task<Result<int>> SaveTestAsync(AdminTestFormDto request, CancellationToken cancellationToken = default)
        {
            // Kiểm tra khóa ngoại Lesson trước khi tạo hoặc cập nhật Test.
            bool lessonExists = await _dbContext.Lessons.AsNoTracking().AnyAsync(x => x.Id == request.LessonId, cancellationToken);

            // Không lưu Test trỏ tới Lesson không tồn tại.
            if (!lessonExists)
            {
                return Result<int>.Failure(new Error("Test.InvalidLesson", "Bài học được chọn không tồn tại."));
            }

            // Biến này nhận entity mới hoặc entity cần cập nhật.
            Test test;

            // Nhánh cập nhật chỉ chạy khi form có ID.
            if (request.Id.HasValue)
            {
                // Tải Questions để kiểm tra cấu trúc FullMock bằng dữ liệu server.
                test = await _dbContext.Tests
                    .Include(x => x.Questions)
                    .FirstOrDefaultAsync(x => x.Id == request.Id.Value, cancellationToken) ?? null!;

                // Trả lỗi khi ID form không còn tồn tại.
                if (test is null)
                {
                    return Result<int>.Failure(new Error("Test.NotFound", "Không tìm thấy bài kiểm tra."));
                }

                // Xác định đề đã có lịch sử làm hay chưa.
                bool hasAttempts = await _dbContext.TestResults.AsNoTracking().AnyAsync(x => x.TestId == test.Id, cancellationToken);

                // Sau khi có lịch sử chỉ Title, Description và IsActive được thay đổi.
                bool changesScoringStructure = test.LessonId != request.LessonId
                    || test.Format != request.Format
                    || test.Mode != request.Mode
                    || test.DurationMinutes != request.DurationMinutes;

                // Chặn mọi thay đổi cấu trúc có thể làm sai lịch sử kết quả.
                if (hasAttempts && changesScoringStructure)
                {
                    return Result<int>.Failure(new Error("Test.StructureInUse", "Đề đã có lượt làm; chỉ được sửa tiêu đề, mô tả và trạng thái hoạt động."));
                }

                // Không đổi format khi đã có câu vì metadata Part/Section có thể không còn hợp lệ.
                bool formatChangedWithQuestions = test.Format != request.Format
                    && test.Questions.Count > 0;

                // Admin phải điều chỉnh câu hỏi ở bản Practice trước khi đổi format.
                if (formatChangedWithQuestions)
                {
                    return Result<int>.Failure(new Error("Test.FormatInUse", "Hãy xóa hoặc điều chỉnh câu hỏi trước khi đổi định dạng bài kiểm tra."));
                }
            }
            else
            {
                // CreatedAt chỉ được gán khi tạo mới.
                test = new Test { CreatedAt = DateTime.UtcNow };

                // Theo dõi entity mới để EF tạo INSERT.
                _dbContext.Tests.Add(test);
            }

            // FullMock chỉ được lưu khi số câu, Part và duration đã đúng toàn bộ.
            if (request.Mode == TestMode.FullMock)
            {
                // Dữ liệu Question được tải từ database và kiểm tra ngay trong SaveTestAsync.
                List<Question> questions = test.Questions.ToList();

                // TOEIC FullMock dùng một timer chung 120 phút.
                if (request.Format == TestFormat.ToeicStyle && request.DurationMinutes != 120)
                {
                    return Result<int>.Failure(new Error("Test.InvalidFullMockDuration", "TOEIC FullMock phải có thời gian 120 phút."));
                }

                // IELTS Listening + Reading dùng timer chung 90 phút trong web app.
                if (request.Format == TestFormat.IeltsStyle && request.DurationMinutes != 90)
                {
                    return Result<int>.Failure(new Error("Test.InvalidFullMockDuration", "IELTS FullMock phải có thời gian 90 phút."));
                }

                // Kiểm tra cấu trúc TOEIC trực tiếp trong nhánh TOEIC.
                if (request.Format == TestFormat.ToeicStyle)
                {
                    int[] expectedCounts = [6, 25, 39, 30, 30, 16, 54];

                    // Mỗi Part phải đúng count công khai.
                    for (int part = 1; part <= expectedCounts.Length; part++)
                    {
                        int actual = questions.Count(x => x.PartNumber == part);
                        if (actual != expectedCounts[part - 1])
                        {
                            return Result<int>.Failure(new Error("Test.InvalidToeicPartCount", $"TOEIC FullMock Part {part} cần {expectedCounts[part - 1]} câu, hiện có {actual}."));
                        }
                    }

                    // Part 1-4 chỉ là ListeningChoice thuộc Listening.
                    if (questions.Any(x => x.PartNumber <= 4 && (x.SectionName != "Listening" || x.QuestionType != QuestionType.ListeningChoice)))
                    {
                        return Result<int>.Failure(new Error("Test.InvalidToeicListening", "TOEIC FullMock Part 1-4 chỉ nhận ListeningChoice thuộc Listening."));
                    }

                    // Part 5-7 chỉ là MultipleChoice thuộc Reading.
                    if (questions.Any(x => x.PartNumber >= 5 && (x.SectionName != "Reading" || x.QuestionType != QuestionType.MultipleChoice)))
                    {
                        return Result<int>.Failure(new Error("Test.InvalidToeicReading", "TOEIC FullMock Part 5-7 chỉ nhận MultipleChoice thuộc Reading."));
                    }

                    // Listening luôn cần audio.
                    if (questions.Any(x => x.PartNumber <= 4 && string.IsNullOrWhiteSpace(x.AudioUrl)))
                    {
                        return Result<int>.Failure(new Error("Test.MissingToeicAudio", "TOEIC FullMock Part 1-4 phải có AudioUrl cho mọi câu."));
                    }

                    // Part 1 luôn cần ảnh.
                    if (questions.Any(x => x.PartNumber == 1 && string.IsNullOrWhiteSpace(x.ImageUrl)))
                    {
                        return Result<int>.Failure(new Error("Test.MissingToeicImage", "TOEIC FullMock Part 1 phải có ImageUrl cho mọi câu."));
                    }

                    // Part 6-7 luôn cần group và passage.
                    if (questions.Any(x => x.PartNumber is 6 or 7
                        && (string.IsNullOrWhiteSpace(x.GroupCode) || string.IsNullOrWhiteSpace(x.ContextText))))
                    {
                        return Result<int>.Failure(new Error("Test.InvalidToeicReadingGroup", "TOEIC FullMock Part 6-7 phải có GroupCode và ContextText."));
                    }

                    // Part 3 phải có đúng 13 group và mỗi group đúng 3 câu.
                    List<IGrouping<string, Question>> partThreeGroups = questions
                        .Where(x => x.PartNumber == 3 && !string.IsNullOrWhiteSpace(x.GroupCode))
                        .GroupBy(x => x.GroupCode!, StringComparer.OrdinalIgnoreCase)
                        .ToList();
                    if (partThreeGroups.Count != 13 || partThreeGroups.Any(group => group.Count() != 3))
                    {
                        return Result<int>.Failure(new Error("Test.InvalidToeicPart3Groups", "TOEIC FullMock Part 3 cần 13 group, mỗi group 3 câu."));
                    }

                    // Part 4 phải có đúng 10 group và mỗi group đúng 3 câu.
                    List<IGrouping<string, Question>> partFourGroups = questions
                        .Where(x => x.PartNumber == 4 && !string.IsNullOrWhiteSpace(x.GroupCode))
                        .GroupBy(x => x.GroupCode!, StringComparer.OrdinalIgnoreCase)
                        .ToList();
                    if (partFourGroups.Count != 10 || partFourGroups.Any(group => group.Count() != 3))
                    {
                        return Result<int>.Failure(new Error("Test.InvalidToeicPart4Groups", "TOEIC FullMock Part 4 cần 10 group, mỗi group 3 câu."));
                    }
                }
                else
                {
                    // IELTS cần đúng 40 Listening và 40 Reading.
                    int listeningCount = questions.Count(x => x.SectionName == "Listening");
                    int readingCount = questions.Count(x => x.SectionName == "Reading");
                    if (listeningCount != 40 || readingCount != 40)
                    {
                        return Result<int>.Failure(new Error("Test.InvalidIeltsSectionCount", $"IELTS FullMock cần 40 Listening và 40 Reading; hiện có {listeningCount} và {readingCount}."));
                    }

                    // Listening chỉ nhận choice/fill và có audio, group.
                    if (questions.Any(x => x.SectionName == "Listening"
                        && (x.QuestionType is not (QuestionType.ListeningChoice or QuestionType.ListeningFill)
                            || string.IsNullOrWhiteSpace(x.AudioUrl)
                            || string.IsNullOrWhiteSpace(x.GroupCode))))
                    {
                        return Result<int>.Failure(new Error("Test.InvalidIeltsListening", "IELTS Listening phải dùng ListeningChoice/ListeningFill và có AudioUrl, GroupCode."));
                    }

                    // Reading không dùng loại Listening và luôn có context, group.
                    if (questions.Any(x => x.SectionName == "Reading"
                        && (x.QuestionType is QuestionType.ListeningChoice or QuestionType.ListeningFill
                            || string.IsNullOrWhiteSpace(x.ContextText)
                            || string.IsNullOrWhiteSpace(x.GroupCode))))
                    {
                        return Result<int>.Failure(new Error("Test.InvalidIeltsReading", "IELTS Reading phải dùng dạng đọc và có ContextText, GroupCode."));
                    }

                    // Mỗi Listening Part 1-4 có đúng 10 câu.
                    for (int part = 1; part <= 4; part++)
                    {
                        int actual = questions.Count(x => x.SectionName == "Listening" && x.PartNumber == part);
                        if (actual != 10)
                        {
                            return Result<int>.Failure(new Error("Test.InvalidIeltsListeningPart", $"IELTS Listening Part {part} cần 10 câu, hiện có {actual}."));
                        }
                    }

                    // Reading chỉ có Passage 1-3 và mỗi passage có ít nhất một câu.
                    bool invalidPassage = questions.Any(x => x.SectionName == "Reading" && x.PartNumber is < 1 or > 3)
                        || Enumerable.Range(1, 3).Any(part => !questions.Any(x => x.SectionName == "Reading" && x.PartNumber == part));
                    if (invalidPassage)
                    {
                        return Result<int>.Failure(new Error("Test.InvalidIeltsReadingPassage", "IELTS Reading FullMock phải tổ chức đủ Passage 1-3."));
                    }
                }
            }

            // Gán Lesson từ form đã được kiểm tra khóa ngoại.
            test.LessonId = request.LessonId;

            // Trim tiêu đề trước khi lưu.
            test.Title = request.Title.Trim();

            // Chuỗi mô tả trắng được lưu thành null.
            test.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();

            // Duration của FullMock đã qua ValidateFullMock; Practice dùng giá trị form.
            test.DurationMinutes = request.DurationMinutes;

            // Lưu format TOEIC-style hoặc IELTS-style.
            test.Format = request.Format;

            // Lưu chế độ Practice hoặc FullMock.
            test.Mode = request.Mode;

            // IsActive quyết định đề có xuất hiện ở danh sách người học hay không.
            test.IsActive = request.IsActive;

            // Thực hiện INSERT/UPDATE duy nhất cho Test.
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Trả ID để Controller điều hướng về dashboard.
            return Result<int>.Success(test.Id);
        }

        public async Task<Result> DeleteTestAsync(int id, CancellationToken cancellationToken = default)
        {
            Test? test = await _dbContext.Tests.Include(x => x.TestResults).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (test is null)
            {
                return Result.Failure(new Error("Test.NotFound", "Không tìm thấy bài kiểm tra."));
            }

            if (test.TestResults.Count > 0)
            {
                return Result.Failure(new Error("Test.InUse", "Không thể xóa bài kiểm tra đã có kết quả."));
            }

            _dbContext.Tests.Remove(test);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result<AdminQuestionListDto>> GetQuestionsAsync(int testId, CancellationToken cancellationToken = default)
        {
            AdminQuestionListDto? dto = await _dbContext.Tests
                .AsNoTracking()
                .Where(x => x.Id == testId)
                .Select(x => new AdminQuestionListDto
                {
                    TestId = x.Id,
                    TestTitle = x.Title,
                    Format = x.Format,
                    Mode = x.Mode,
                    Items = x.Questions
                        .OrderBy(question => question.Order)
                        .Select(question => new AdminQuestionItemDto
                        {
                            Id = question.Id,
                            Content = question.Content,
                            QuestionType = question.QuestionType,
                            SectionName = question.SectionName,
                            PartNumber = question.PartNumber,
                            Order = question.Order,
                            AnswerCount = question.Answers.Count
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            return dto is null
                ? Result<AdminQuestionListDto>.Failure(new Error("Test.NotFound", "Không tìm thấy bài kiểm tra."))
                : Result<AdminQuestionListDto>.Success(dto);
        }

        public async Task<Result<AdminQuestionFormDto>> GetQuestionFormAsync(int testId, int? id, CancellationToken cancellationToken = default)
        {
            bool testExists = await _dbContext.Tests.AsNoTracking().AnyAsync(x => x.Id == testId, cancellationToken);
            if (!testExists)
            {
                return Result<AdminQuestionFormDto>.Failure(new Error("Test.NotFound", "Không tìm thấy bài kiểm tra."));
            }

            if (!id.HasValue)
            {
                int nextOrder = (await _dbContext.Questions.AsNoTracking().Where(x => x.TestId == testId).MaxAsync(x => (int?)x.Order, cancellationToken) ?? 0) + 1;
                return Result<AdminQuestionFormDto>.Success(new AdminQuestionFormDto
                {
                    TestId = testId,
                    Order = nextOrder,
                    Answers = Enumerable.Range(0, 4).Select(_ => new AdminAnswerInputDto()).ToList()
                });
            }

            AdminQuestionFormDto? dto = await _dbContext.Questions
                .AsNoTracking()
                .Where(x => x.Id == id.Value && x.TestId == testId)
                .Select(x => new AdminQuestionFormDto
                {
                    Id = x.Id,
                    TestId = x.TestId,
                    Content = x.Content,
                    QuestionType = x.QuestionType,
                    SectionName = x.SectionName,
                    PartNumber = x.PartNumber,
                    Order = x.Order,
                    GroupCode = x.GroupCode,
                    ContextText = x.ContextText,
                    ImageUrl = x.ImageUrl,
                    AudioUrl = x.AudioUrl,
                    Instruction = x.Instruction,
                    MaxWords = x.MaxWords,
                    Answers = x.Answers.OrderBy(answer => answer.Id).Select(answer => new AdminAnswerInputDto { Content = answer.Content, IsCorrect = answer.IsCorrect }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            return dto is null
                ? Result<AdminQuestionFormDto>.Failure(new Error("Question.NotFound", "Không tìm thấy câu hỏi trong bài kiểm tra."))
                : Result<AdminQuestionFormDto>.Success(dto);
        }

        public async Task<Result> SaveQuestionAsync(AdminQuestionFormDto request, CancellationToken cancellationToken = default)
        {
            // TestId phải được xác nhận từ database trước khi dùng để lưu Question.
            Test? test = await _dbContext.Tests.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.TestId, cancellationToken);

            // Ngăn request sửa chéo vào một Test không tồn tại.
            if (test is null)
            {
                return Result.Failure(new Error("Test.NotFound", "Không tìm thấy bài kiểm tra."));
            }

            // FullMock đã được xác nhận cấu trúc nên phải chuyển về Practice trước khi sửa câu.
            if (test.Mode == TestMode.FullMock)
            {
                return Result.Failure(new Error("Question.FullMockLocked", "Hãy chuyển đề về Practice trước khi thay đổi cấu trúc câu hỏi."));
            }

            // Không cho thay đổi bất kỳ nội dung chấm nào sau khi đề đã có lượt làm.
            bool hasAttempts = await _dbContext.TestResults.AsNoTracking().AnyAsync(x => x.TestId == request.TestId, cancellationToken);
            if (hasAttempts)
            {
                return Result.Failure(new Error("Question.InUse", "Không thể thay đổi câu hỏi của bài kiểm tra đã có kết quả."));
            }

            // Loại dòng đáp án trống và trim nội dung trước khi validation.
            List<AdminAnswerInputDto> answers = request.Answers
                .Where(x => !string.IsNullOrWhiteSpace(x.Content))
                .Select(x => new AdminAnswerInputDto { Content = x.Content!.Trim(), IsCorrect = x.IsCorrect })
                .ToList();

            // Chuẩn hóa Section ngay trong hàm public để Service không cần hàm private phụ trợ.
            string section = request.SectionName.Equals("Listening", StringComparison.OrdinalIgnoreCase) ? "Listening" : "Reading";

            // Biến này cho biết câu hiện tại thuộc phần nghe hay phần đọc.
            bool isListening = section == "Listening";

            // Hai loại completion là câu nhập nội dung dạng chữ.
            bool isText = request.QuestionType is QuestionType.FillBlank or QuestionType.ListeningFill;

            // Bốn loại còn lại là câu chọn một đáp án từ danh sách.
            bool isChoice = request.QuestionType is QuestionType.MultipleChoice or QuestionType.ListeningChoice or QuestionType.TrueFalseNotGiven or QuestionType.YesNoNotGiven;

            // TOEIC L&R chỉ cho phép Listening Part 1-4 và Reading Part 5-7.
            if (test.Format == TestFormat.ToeicStyle)
            {
                // Kiểm tra Part dựa trên Section đã được chuẩn hóa ở phía server.
                bool validPart = (isListening && request.PartNumber is >= 1 and <= 4)
                    || (!isListening && request.PartNumber is >= 5 and <= 7);

                // Dừng lưu khi Part không đúng cấu trúc TOEIC.
                if (!validPart)
                {
                    return Result.Failure(new Error("Question.InvalidPart", "TOEIC-style dùng Listening Part 1-4 và Reading Part 5-7."));
                }

                // TOEIC Listening dùng ListeningChoice còn Reading dùng MultipleChoice.
                QuestionType expectedType = isListening ? QuestionType.ListeningChoice : QuestionType.MultipleChoice;

                // Dừng lưu khi loại câu hỏi không khớp phần thi.
                if (request.QuestionType != expectedType)
                {
                    return Result.Failure(new Error("Question.InvalidToeicType", "TOEIC-style dùng ListeningChoice cho Part 1-4 và MultipleChoice cho Part 5-7."));
                }

                // Các phần có nội dung dùng chung phải có mã nhóm.
                if (request.PartNumber is 3 or 4 or 6 or 7 && string.IsNullOrWhiteSpace(request.GroupCode))
                {
                    return Result.Failure(new Error("Question.GroupRequired", "TOEIC Part 3, 4, 6 và 7 phải có GroupCode."));
                }

                // Reading Part 6-7 phải có đoạn văn dùng chung.
                if (request.PartNumber is 6 or 7 && string.IsNullOrWhiteSpace(request.ContextText))
                {
                    return Result.Failure(new Error("Question.ContextRequired", "TOEIC Reading Part 6 và 7 phải có ContextText."));
                }
            }
            else
            {
                // IELTS Listening dùng Part 1-4 còn Reading dùng Passage 1-3.
                bool validSection = (isListening && request.PartNumber is >= 1 and <= 4)
                    || (!isListening && request.PartNumber is >= 1 and <= 3);

                // Dừng lưu khi Part hoặc Passage nằm ngoài phạm vi IELTS.
                if (!validSection)
                {
                    return Result.Failure(new Error("Question.InvalidSection", "IELTS-style dùng Listening Section 1-4 và Reading Section 1-3."));
                }

                // IELTS Listening chỉ dùng dạng chọn hoặc điền dành cho phần nghe.
                bool validListeningType = request.QuestionType is QuestionType.ListeningChoice or QuestionType.ListeningFill;

                // IELTS Reading dùng các dạng câu hỏi đọc hiểu.
                bool validReadingType = request.QuestionType is QuestionType.MultipleChoice or QuestionType.FillBlank or QuestionType.TrueFalseNotGiven or QuestionType.YesNoNotGiven;

                // Dừng lưu khi loại câu không phù hợp với Section.
                if ((isListening && !validListeningType) || (!isListening && !validReadingType))
                {
                    return Result.Failure(new Error("Question.InvalidIeltsType", "Loại câu hỏi không phù hợp với Section IELTS đã chọn."));
                }

                // Mỗi recording hoặc passage IELTS phải có mã nhóm.
                if (string.IsNullOrWhiteSpace(request.GroupCode))
                {
                    return Result.Failure(new Error("Question.GroupRequired", "Câu IELTS phải có GroupCode của recording hoặc passage."));
                }

                // Reading Passage phải có đoạn văn để người học đọc.
                if (!isListening && string.IsNullOrWhiteSpace(request.ContextText))
                {
                    return Result.Failure(new Error("Question.ContextRequired", "IELTS Reading phải có ContextText của passage."));
                }
            }

            // Mọi câu Listening cần đường dẫn âm thanh.
            if (isListening && string.IsNullOrWhiteSpace(request.AudioUrl))
            {
                return Result.Failure(new Error("Question.AudioRequired", "Câu Listening phải có URL audio."));
            }

            // TOEIC Part 1 cần hình ảnh để người học chọn câu mô tả phù hợp.
            if (test.Format == TestFormat.ToeicStyle && isListening && request.PartNumber == 1 && string.IsNullOrWhiteSpace(request.ImageUrl))
            {
                return Result.Failure(new Error("Question.ImageRequired", "TOEIC-style Part 1 phải có hình ảnh."));
            }

            // MaxWords chỉ được dùng cho câu trả lời dạng chữ.
            if (request.MaxWords.HasValue && !isText)
            {
                return Result.Failure(new Error("Question.InvalidMaxWords", "Chỉ câu FillBlank hoặc ListeningFill mới dùng MaxWords."));
            }

            // Khi giới hạn số từ thì đề phải có hướng dẫn tương ứng.
            if (request.MaxWords.HasValue && string.IsNullOrWhiteSpace(request.Instruction))
            {
                return Result.Failure(new Error("Question.InstructionRequired", "Câu có MaxWords phải có Instruction."));
            }

            // Câu lựa chọn cần ít nhất hai đáp án và chỉ một đáp án đúng.
            if (isChoice && (answers.Count < 2 || answers.Count(x => x.IsCorrect) != 1))
            {
                return Result.Failure(new Error("Question.InvalidAnswers", "Câu lựa chọn cần ít nhất hai đáp án và đúng một đáp án đúng."));
            }

            // Câu điền cần ít nhất một chuỗi đáp án được chấp nhận.
            if (!isChoice && (answers.Count == 0 || answers.All(x => !x.IsCorrect)))
            {
                return Result.Failure(new Error("Question.InvalidAnswers", "Câu điền cần ít nhất một đáp án đúng."));
            }

            // Tạo tập option để kiểm tra các dạng có lựa chọn cố định mà không cần hàm private.
            HashSet<string> actualOptions = answers.Select(x => x.Content!.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

            // TRUE/FALSE/NOT GIVEN phải có đúng ba option, không thiếu và không thừa.
            if (request.QuestionType == QuestionType.TrueFalseNotGiven)
            {
                // Đây là tập option đúng theo dạng câu hỏi IELTS.
                string[] expectedOptions = ["TRUE", "FALSE", "NOT GIVEN"];

                // SetEquals kiểm tra nội dung còn Count kiểm tra không có option trùng lặp.
                bool hasExactOptions = answers.Count == expectedOptions.Length
                    && actualOptions.Count == expectedOptions.Length
                    && actualOptions.SetEquals(expectedOptions);

                // Dừng lưu nếu danh sách option không chính xác.
                if (!hasExactOptions)
                {
                    return Result.Failure(new Error("Question.InvalidFixedAnswers", "Dạng TRUE/FALSE/NOT GIVEN phải có đúng ba lựa chọn cố định."));
                }
            }

            // YES/NO/NOT GIVEN cũng phải có đúng ba option cố định.
            if (request.QuestionType == QuestionType.YesNoNotGiven)
            {
                // Đây là tập option đúng theo dạng câu hỏi IELTS.
                string[] expectedOptions = ["YES", "NO", "NOT GIVEN"];

                // SetEquals kiểm tra nội dung còn Count kiểm tra không có option trùng lặp.
                bool hasExactOptions = answers.Count == expectedOptions.Length
                    && actualOptions.Count == expectedOptions.Length
                    && actualOptions.SetEquals(expectedOptions);

                // Dừng lưu nếu danh sách option không chính xác.
                if (!hasExactOptions)
                {
                    return Result.Failure(new Error("Question.InvalidFixedAnswers", "Dạng YES/NO/NOT GIVEN phải có đúng ba lựa chọn cố định."));
                }
            }

            // Kiểm tra Order bằng TestId server-side, không dựa vào UI.
            bool duplicateOrder = await _dbContext.Questions.AsNoTracking().AnyAsync(x => x.TestId == request.TestId && x.Order == request.Order && (!request.Id.HasValue || x.Id != request.Id.Value), cancellationToken);

            // Unique Order giúp thứ tự hiển thị/chấm không mơ hồ.
            if (duplicateOrder)
            {
                return Result.Failure(new Error("Question.DuplicateOrder", "Thứ tự câu hỏi đã được sử dụng trong bài kiểm tra."));
            }

            // Biến nhận câu mới hoặc câu đang sửa.
            Question question;

            // Nhánh sửa chỉ tìm câu đồng thời khớp cả QuestionId và TestId.
            if (request.Id.HasValue)
            {
                question = await _dbContext.Questions.Include(x => x.Answers).FirstOrDefaultAsync(x => x.Id == request.Id.Value && x.TestId == request.TestId, cancellationToken) ?? null!;

                // Không cho ID câu của Test khác đi qua.
                if (question is null)
                {
                    return Result.Failure(new Error("Question.NotFound", "Không tìm thấy câu hỏi trong bài kiểm tra."));
                }

                // Xóa các Answer cũ trong cùng lần SaveChanges rồi tạo danh sách mới từ form đã validate.
                _dbContext.Answers.RemoveRange(question.Answers);
            }
            else
            {
                // Câu mới luôn gắn TestId đã kiểm tra ở đầu hàm.
                question = new Question { TestId = request.TestId };

                // Đưa Question mới vào change tracker.
                _dbContext.Questions.Add(question);
            }

            // Các phép gán dưới đây chỉ chạy sau khi toàn bộ validation thành công.
            question.Content = request.Content.Trim();
            question.QuestionType = request.QuestionType;
            question.SectionName = section;
            question.PartNumber = request.PartNumber;
            question.Order = request.Order;
            question.GroupCode = string.IsNullOrWhiteSpace(request.GroupCode) ? null : request.GroupCode.Trim();
            question.ContextText = string.IsNullOrWhiteSpace(request.ContextText) ? null : request.ContextText.Trim();
            question.ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim();
            question.AudioUrl = string.IsNullOrWhiteSpace(request.AudioUrl) ? null : request.AudioUrl.Trim();
            question.Instruction = string.IsNullOrWhiteSpace(request.Instruction) ? null : request.Instruction.Trim();
            question.MaxWords = request.MaxWords;

            // Chỉ các Answer đã lọc và đánh dấu đúng hợp lệ mới được tạo.
            question.Answers = answers.Select(x => new Answer { Content = x.Content!, IsCorrect = x.IsCorrect }).ToList();

            // EF lưu Question và Answer trong một SaveChanges transaction.
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Báo thành công cho Controller.
            return Result.Success();
        }

        public async Task<Result> DeleteQuestionAsync(int id, CancellationToken cancellationToken = default)
        {
            Question? question = await _dbContext.Questions.Include(x => x.Test).ThenInclude(x => x.TestResults).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (question is null)
            {
                return Result.Failure(new Error("Question.NotFound", "Không tìm thấy câu hỏi."));
            }

            if (question.Test.TestResults.Count > 0)
            {
                return Result.Failure(new Error("Question.InUse", "Không thể xóa câu hỏi của bài kiểm tra đã có kết quả."));
            }

            if (question.Test.Mode == TestMode.FullMock)
            {
                return Result.Failure(new Error("Question.FullMockLocked", "Hãy chuyển đề về Practice trước khi xóa câu hỏi."));
            }

            _dbContext.Questions.Remove(question);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

    }
}
