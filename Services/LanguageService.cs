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

        public async Task<Result<AdminLanguageListDto>> GetAsync(string? search, CancellationToken cancellationToken = default)
        {
            string normalizedSearch = search?.Trim() ?? string.Empty;
            IQueryable<Language> query = _dbContext.Languages.AsNoTracking();

            if (normalizedSearch.Length > 0)
            {
                query = query.Where(x => x.Name.Contains(normalizedSearch) || x.Code.Contains(normalizedSearch));
            }

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

        public async Task<Result<AdminLanguageFormDto>> GetFormAsync(int? id, CancellationToken cancellationToken = default)
        {
            if (!id.HasValue)
            {
                return Result<AdminLanguageFormDto>.Success(new AdminLanguageFormDto());
            }

            AdminLanguageFormDto? dto = await _dbContext.Languages
                .AsNoTracking()
                .Where(x => x.Id == id.Value)
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

        public async Task<Result<int>> SaveAsync(AdminLanguageFormDto request, CancellationToken cancellationToken = default)
        {
            string name = request.Name.Trim();
            string code = request.Code.Trim().ToLowerInvariant();

            bool duplicateCode = await _dbContext.Languages
                .AnyAsync(x => x.Code == code && (!request.Id.HasValue || x.Id != request.Id.Value), cancellationToken);

            if (duplicateCode)
            {
                return Result<int>.Failure(new Error("Language.DuplicateCode", "Mã ngôn ngữ đã tồn tại."));
            }

            Language language;
            if (request.Id.HasValue)
            {
                language = await _dbContext.Languages.FirstOrDefaultAsync(x => x.Id == request.Id.Value, cancellationToken)
                    ?? null!;

                if (language is null)
                {
                    return Result<int>.Failure(new Error("Language.NotFound", "Không tìm thấy ngôn ngữ."));
                }
            }
            else
            {
                language = new Language();
                _dbContext.Languages.Add(language);
            }

            language.Name = name;
            language.Code = code;
            language.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(language.Id);
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
