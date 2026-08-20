using Microsoft.EntityFrameworkCore;
using SKDJK.Data;
using SKDJK.Dtos;
using SKDJK.Models;
using SKDJK.Models.commons;
using SKDJK.Services.Interfaces;

namespace SKDJK.Services
{
    // Thực hiện CRUD Language trực tiếp bằng ApplicationDbContext theo kiến trúc hiện có.
    public sealed class LanguageService : ILanguageService
    {
        private readonly ApplicationDbContext _dbContext;

        public LanguageService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<AdminLanguageListDto>> GetAllAsync(string? search, CancellationToken cancellationToken = default)
        {
            // Chuẩn hóa từ khóa để tìm kiếm không bị ảnh hưởng bởi khoảng trắng hai đầu.
            string normalizedSearch = search?.Trim() ?? string.Empty;

            // Truy vấn chỉ đọc giúp EF Core không theo dõi Entity không cần cập nhật.
            IQueryable<Language> query = _dbContext.Languages.AsNoTracking();

            // Chỉ thêm điều kiện khi người quản trị thực sự nhập từ khóa.
            if (normalizedSearch.Length > 0)
            {
                query = query.Where(x => x.Name.Contains(normalizedSearch) || x.Code.Contains(normalizedSearch));
            }

            // Service project Entity thành DTO danh sách trước khi trả cho Controller.
            AdminLanguageListDto dto = new()
            {
                Search = search,
                Items = await query
                    .OrderBy(x => x.Name)
                    .Select(x => new AdminLanguageItemDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Code = x.Code,
                        Description = x.Description,
                        TopicCount = x.Topics.Count
                    })
                    .ToListAsync(cancellationToken)
            };

            return Result<AdminLanguageListDto>.Success(dto);
        }

        public async Task<Result<AdminLanguageFormDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            // Đọc đúng một Language thành DTO, không trả Entity ra Controller.
            AdminLanguageFormDto? dto = await _dbContext.Languages
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new AdminLanguageFormDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code,
                    Description = x.Description
                })
                .FirstOrDefaultAsync(cancellationToken);

            return dto is null
                ? Result<AdminLanguageFormDto>.Failure(new Error("Language.NotFound", "Không tìm thấy ngôn ngữ."))
                : Result<AdminLanguageFormDto>.Success(dto);
        }

        public async Task<Result<int>> CreateAsync(CreateLanguageDto request, CancellationToken cancellationToken = default)
        {
            // Chuẩn hóa dữ liệu đầu vào trước khi kiểm tra và lưu.
            string name = (request.Name ?? string.Empty).Trim();
            string code = (request.Code ?? string.Empty).Trim().ToLowerInvariant();

            // Service vẫn kiểm tra input để an toàn khi được gọi ngoài Razor Controller.
            if (name.Length == 0 || name.Length > 50)
            {
                return Result<int>.Failure(new Error("Language.InvalidName", "Tên ngôn ngữ phải có từ 1 đến 50 ký tự."));
            }

            // Mã ngôn ngữ phải đúng giới hạn cột và chỉ dùng chữ cái hoặc dấu gạch ngang.
            if (code.Length == 0 || code.Length > 10 || code.Any(character => (character < 'a' || character > 'z') && character != '-'))
            {
                return Result<int>.Failure(new Error("Language.InvalidCode", "Mã ngôn ngữ chỉ gồm chữ cái, dấu gạch ngang và tối đa 10 ký tự."));
            }

            // CREATE kiểm tra trùng code trên toàn bộ bảng vì chưa có bản ghi hiện tại để loại trừ.
            bool duplicateCode = await _dbContext.Languages
                .AnyAsync(x => x.Code == code, cancellationToken);

            // Không cho database ném lỗi unique index khi có thể trả lỗi nghiệp vụ rõ ràng.
            if (duplicateCode)
            {
                return Result<int>.Failure(new Error("Language.DuplicateCode", "Mã ngôn ngữ đã tồn tại."));
            }

            // CreateAsync luôn tạo Entity mới và không chứa nhánh cập nhật.
            Language language = new()
            {
                Name = name,
                Code = code,
                Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim()
            };

            // Đưa Entity mới vào change tracker.
            _dbContext.Languages.Add(language);

            // Một SaveChangesAsync là đủ atomic cho thao tác thêm một Entity.
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Trả Id do database tạo để Controller có thể redirect nếu cần.
            return Result<int>.Success(language.Id);
        }

        public async Task<Result> UpdateAsync(int id, UpdateLanguageDto request, CancellationToken cancellationToken = default)
        {
            // UPDATE phải tìm bản ghi theo Id riêng, không đọc Id từ DTO.
            Language? language = await _dbContext.Languages
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            // Không biến Update thành Create khi Id không tồn tại.
            if (language is null)
            {
                return Result.Failure(new Error("Language.NotFound", "Không tìm thấy ngôn ngữ."));
            }

            // Chuẩn hóa dữ liệu cập nhật theo cùng convention với Create.
            string name = (request.Name ?? string.Empty).Trim();
            string code = (request.Code ?? string.Empty).Trim().ToLowerInvariant();

            // Kiểm tra tên trước khi gán vào Entity đang được theo dõi.
            if (name.Length == 0 || name.Length > 50)
            {
                return Result.Failure(new Error("Language.InvalidName", "Tên ngôn ngữ phải có từ 1 đến 50 ký tự."));
            }

            // Kiểm tra định dạng code tại Service thay vì chỉ tin ModelState của Razor.
            if (code.Length == 0 || code.Length > 10 || code.Any(character => (character < 'a' || character > 'z') && character != '-'))
            {
                return Result.Failure(new Error("Language.InvalidCode", "Mã ngôn ngữ chỉ gồm chữ cái, dấu gạch ngang và tối đa 10 ký tự."));
            }

            // Loại trừ chính Id hiện tại khi kiểm tra code trùng.
            bool duplicateCode = await _dbContext.Languages
                .AnyAsync(x => x.Code == code && x.Id != id, cancellationToken);

            // Trả lỗi nghiệp vụ nếu một Language khác đã sử dụng code.
            if (duplicateCode)
            {
                return Result.Failure(new Error("Language.DuplicateCode", "Mã ngôn ngữ đã tồn tại."));
            }

            // Gán các trường được phép cập nhật lên Entity hiện tại.
            language.Name = name;
            language.Code = code;
            language.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();

            // Lưu duy nhất thao tác cập nhật Language.
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Update thành công không cần trả lại Id vì Id đã nằm trên URL.
            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            Language? language = await _dbContext.Languages
                .Include(x => x.Topics)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (language is null)
            {
                return Result.Failure(new Error("Language.NotFound", "Không tìm thấy ngôn ngữ."));
            }

            if (language.Topics.Count > 0)
            {
                return Result.Failure(new Error("Language.InUse", "Không thể xóa ngôn ngữ đang có chủ đề."));
            }

            _dbContext.Languages.Remove(language);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
