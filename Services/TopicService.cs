using Microsoft.EntityFrameworkCore;
using SKDJK.Data;
using SKDJK.Dtos;
using SKDJK.Models;
using SKDJK.Models.commons;
using SKDJK.Services.Interfaces;

namespace SKDJK.Services
{
    // TopicService chỉ dùng Entity nội bộ và DTO tại biên Controller.
    public sealed class TopicService : ITopicService
    {
        private readonly ApplicationDbContext _dbContext;

        public TopicService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<TopicListDto>> GetAllAsync(string? search, int? languageId, string? level, int page = 1, int pageSize = 12, CancellationToken cancellationToken = default)
        {
            // Chuẩn hóa phân trang ngay trong hàm thay vì gọi helper private.
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 48);

            // Khởi tạo truy vấn chỉ đọc.
            IQueryable<Topic> query = _dbContext.Topics.AsNoTracking();

            // Áp dụng từng filter trực tiếp để toàn bộ logic của action nằm cùng một nơi.
            if (!string.IsNullOrWhiteSpace(search))
            {
                string value = search.Trim();
                query = query.Where(x => x.Name.Contains(value));
            }

            if (languageId.HasValue)
            {
                query = query.Where(x => x.LanguageId == languageId.Value);
            }

            if (!string.IsNullOrWhiteSpace(level))
            {
                string value = level.Trim();
                query = query.Where(x => x.Level == value);
            }

            // Đếm trước khi Skip/Take để Controller tính tổng trang trên ViewModel.
            int totalItems = await query.CountAsync(cancellationToken);

            // Project Entity thành DTO, không project thành ViewModel trong Service.
            List<TopicCardDto> topics = await query
                .OrderBy(x => x.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new TopicCardDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Level = x.Level,
                    LanguageName = x.Language.Name,
                    Description = x.Description,
                    ImageUrl = x.ImageUrl
                })
                .ToListAsync(cancellationToken);

            // Truy vấn option ngôn ngữ trực tiếp trong phương thức theo yêu cầu không dùng hàm private.
            List<LanguageOptionDto> languages = await _dbContext.Languages
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new LanguageOptionDto { Id = x.Id, Name = x.Name })
                .ToListAsync(cancellationToken);

            // Tạo DTO hoàn chỉnh để Controller ánh xạ sang TopicListViewModel.
            TopicListDto dto = new()
            {
                Search = search,
                LanguageId = languageId,
                Level = level,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                Topics = topics,
                Languages = languages,
                Levels = await _dbContext.Topics.AsNoTracking().Select(x => x.Level).Distinct().OrderBy(x => x).ToListAsync(cancellationToken)
            };

            return Result<TopicListDto>.Success(dto);
        }

        public async Task<Result<TopicDetailsDto>> GetByIdAsync(int id, int? userId = null, CancellationToken cancellationToken = default)
        {
            // Truy vấn chi tiết và tiến độ trực tiếp thành DTO.
            TopicDetailsDto? dto = await _dbContext.Topics
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new TopicDetailsDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Level = x.Level,
                    LanguageName = x.Language.Name,
                    Description = x.Description,
                    ImageUrl = x.ImageUrl,
                    Lessons = x.Lessons
                        .OrderBy(lesson => lesson.Id)
                        .Select(lesson => new TopicLessonDto
                        {
                            Id = lesson.Id,
                            Title = lesson.Title,
                            Description = lesson.Description,
                            CompletionPercent = userId.HasValue
                                ? lesson.LearningProgresses
                                    .Where(progress => progress.UserId == userId.Value)
                                    .Select(progress => progress.CompletionPercent)
                                    .FirstOrDefault()
                                : 0
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            return dto is null
                ? Result<TopicDetailsDto>.Failure(new Error("Topic.NotFound", "Không tìm thấy chủ đề."))
                : Result<TopicDetailsDto>.Success(dto);
        }

        public async Task<Result<AdminTopicListDto>> GetAdminAsync(string? search, int? languageId, string? level, CancellationToken cancellationToken = default)
        {
            // Dashboard admin bắt đầu từ truy vấn chỉ đọc.
            IQueryable<Topic> query = _dbContext.Topics.AsNoTracking();

            // Lặp filter ở phương thức công khai để không cần ApplyFilters private.
            if (!string.IsNullOrWhiteSpace(search))
            {
                string value = search.Trim();
                query = query.Where(x => x.Name.Contains(value));
            }

            if (languageId.HasValue)
            {
                query = query.Where(x => x.LanguageId == languageId.Value);
            }

            if (!string.IsNullOrWhiteSpace(level))
            {
                string value = level.Trim();
                query = query.Where(x => x.Level == value);
            }

            // Lấy danh sách Topic dưới dạng DTO.
            List<TopicCardDto> items = await query
                .OrderBy(x => x.Language.Name)
                .ThenBy(x => x.Name)
                .Select(x => new TopicCardDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    LanguageName = x.Language.Name,
                    Level = x.Level,
                    Description = x.Description,
                    ImageUrl = x.ImageUrl
                })
                .ToListAsync(cancellationToken);

            // Lấy options ngôn ngữ trong chính hàm GetAdminAsync.
            List<LanguageOptionDto> languages = await _dbContext.Languages
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new LanguageOptionDto { Id = x.Id, Name = x.Name })
                .ToListAsync(cancellationToken);

            AdminTopicListDto dto = new()
            {
                Search = search,
                LanguageId = languageId,
                Level = level,
                Languages = languages,
                Items = items
            };

            return Result<AdminTopicListDto>.Success(dto);
        }

        public async Task<Result<AdminTopicFormDto>> GetFormAsync(int? id, CancellationToken cancellationToken = default)
        {
            // Tạo DTO mới hoặc tải DTO đang sửa.
            AdminTopicFormDto dto;
            if (id.HasValue)
            {
                dto = await _dbContext.Topics
                    .AsNoTracking()
                    .Where(x => x.Id == id.Value)
                    .Select(x => new AdminTopicFormDto
                    {
                        Id = x.Id,
                        LanguageId = x.LanguageId,
                        Name = x.Name,
                        Level = x.Level,
                        Description = x.Description,
                        ImageUrl = x.ImageUrl
                    })
                    .FirstOrDefaultAsync(cancellationToken) ?? null!;

                if (dto is null)
                {
                    return Result<AdminTopicFormDto>.Failure(new Error("Topic.NotFound", "Không tìm thấy chủ đề."));
                }
            }
            else
            {
                dto = new AdminTopicFormDto();
            }

            // Options được tải ngay trong phương thức form.
            dto.Languages = await _dbContext.Languages
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new LanguageOptionDto { Id = x.Id, Name = x.Name })
                .ToListAsync(cancellationToken);

            return Result<AdminTopicFormDto>.Success(dto);
        }

        public async Task<Result<int>> CreateAsync(CreateTopicDto request, CancellationToken cancellationToken = default)
        {
            // Xác nhận LanguageId tồn tại trước khi tạo Topic có khóa ngoại này.
            bool languageExists = await _dbContext.Languages
                .AsNoTracking()
                .AnyAsync(x => x.Id == request.LanguageId, cancellationToken);

            // Không để database tự ném lỗi khóa ngoại khó hiểu cho người dùng.
            if (!languageExists)
            {
                return Result<int>.Failure(new Error("Topic.InvalidLanguage", "Ngôn ngữ được chọn không tồn tại."));
            }

            // Chuẩn hóa chuỗi trước khi validation và lưu.
            string name = (request.Name ?? string.Empty).Trim();
            string level = (request.Level ?? string.Empty).Trim();
            string? description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
            string? imageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim();

            // Tên Topic phải phù hợp giới hạn cột hiện tại.
            if (name.Length == 0 || name.Length > 100)
            {
                return Result<int>.Failure(new Error("Topic.InvalidName", "Tên chủ đề phải có từ 1 đến 100 ký tự."));
            }

            // Level giữ đúng ba giá trị đang dùng ở Razor và không tạo bảng Level mới.
            string[] allowedLevels = ["Cơ bản", "Trung cấp", "Nâng cao"];
            string? normalizedLevel = allowedLevels.FirstOrDefault(value => value.Equals(level, StringComparison.OrdinalIgnoreCase));
            if (normalizedLevel is null)
            {
                return Result<int>.Failure(new Error("Topic.InvalidLevel", "Cấp độ phải là Cơ bản, Trung cấp hoặc Nâng cao."));
            }

            // Description phải tuân theo độ dài cột hiện tại thay vì tạo migration mới.
            if (description?.Length > 255)
            {
                return Result<int>.Failure(new Error("Topic.InvalidDescription", "Mô tả chủ đề không được vượt quá 255 ký tự."));
            }

            // CreateAsync luôn khởi tạo Entity mới và không chứa nhánh Update.
            Topic topic = new()
            {
                LanguageId = request.LanguageId,
                Name = name,
                Level = normalizedLevel,
                Description = description,
                ImageUrl = imageUrl
            };

            // Đưa Topic mới vào change tracker.
            _dbContext.Topics.Add(topic);

            // CRUD một Entity chỉ cần một SaveChangesAsync, không cần transaction thủ công.
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Trả Id do database tạo cho Controller.
            return Result<int>.Success(topic.Id);
        }

        public async Task<Result> UpdateAsync(int id, UpdateTopicDto request, CancellationToken cancellationToken = default)
        {
            // Update luôn tìm Topic theo Id URL trước và không tạo mới khi không thấy.
            Topic? topic = await _dbContext.Topics
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            // Id không tồn tại là lỗi NotFound, không phải lệnh Create.
            if (topic is null)
            {
                return Result.Failure(new Error("Topic.NotFound", "Không tìm thấy chủ đề."));
            }

            // Xác nhận LanguageId mới tồn tại trước khi cập nhật khóa ngoại.
            bool languageExists = await _dbContext.Languages
                .AsNoTracking()
                .AnyAsync(x => x.Id == request.LanguageId, cancellationToken);

            // Giữ nguyên Topic nếu người dùng chọn ngôn ngữ không hợp lệ.
            if (!languageExists)
            {
                return Result.Failure(new Error("Topic.InvalidLanguage", "Ngôn ngữ được chọn không tồn tại."));
            }

            // Chuẩn hóa chuỗi theo cùng convention với CreateAsync.
            string name = (request.Name ?? string.Empty).Trim();
            string level = (request.Level ?? string.Empty).Trim();
            string? description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
            string? imageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim();

            // Không ghi tên rỗng hoặc dài hơn cấu hình Entity hiện tại.
            if (name.Length == 0 || name.Length > 100)
            {
                return Result.Failure(new Error("Topic.InvalidName", "Tên chủ đề phải có từ 1 đến 100 ký tự."));
            }

            // Chuẩn hóa Level về đúng cách viết đang dùng trong database.
            string[] allowedLevels = ["Cơ bản", "Trung cấp", "Nâng cao"];
            string? normalizedLevel = allowedLevels.FirstOrDefault(value => value.Equals(level, StringComparison.OrdinalIgnoreCase));
            if (normalizedLevel is null)
            {
                return Result.Failure(new Error("Topic.InvalidLevel", "Cấp độ phải là Cơ bản, Trung cấp hoặc Nâng cao."));
            }

            // Chặn Description vượt độ dài cột để không cần thay đổi schema.
            if (description?.Length > 255)
            {
                return Result.Failure(new Error("Topic.InvalidDescription", "Mô tả chủ đề không được vượt quá 255 ký tự."));
            }

            // Chỉ cập nhật các trường được DTO cho phép.
            topic.LanguageId = request.LanguageId;
            topic.Name = name;
            topic.Level = normalizedLevel;
            topic.Description = description;
            topic.ImageUrl = imageUrl;

            // Lưu duy nhất thao tác Update hiện tại.
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Update không cần trả Id vì Controller đã có Id trên route.
            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            // Tải Lessons để chặn xóa Topic đang được dùng.
            Topic? topic = await _dbContext.Topics.Include(x => x.Lessons).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (topic is null)
            {
                return Result.Failure(new Error("Topic.NotFound", "Không tìm thấy chủ đề."));
            }

            if (topic.Lessons.Count > 0)
            {
                return Result.Failure(new Error("Topic.InUse", "Không thể xóa chủ đề đang có bài học."));
            }

            _dbContext.Topics.Remove(topic);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
