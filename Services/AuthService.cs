using Microsoft.EntityFrameworkCore;
using SKDJK.Data;
using SKDJK.Dtos;
using SKDJK.Models;
using SKDJK.Models.commons;
using SKDJK.Models.enums;
using SKDJK.Services.Interfaces;

namespace SKDJK.Services
{
    // AuthService nhận DTO từ Controller và không biết Login/Register ViewModel.
    public sealed class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _dbContext;

        public AuthService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<AuthenticatedUserDto>> LoginAsync(LoginRequestDto request)
        {
            // Service vẫn kiểm tra dữ liệu để không phụ thuộc hoàn toàn vào ModelState của UI.
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return Result<AuthenticatedUserDto>.Failure(Error.InvalidInput);
            }

            // Chuẩn hóa email trước khi truy vấn.
            string email = request.Email.Trim().ToLowerInvariant();

            // Tải User và Role cần để tạo authentication claim.
            User? user = await _dbContext.Users
                .Include(x => x.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == email);

            // Không phân biệt email sai và mật khẩu sai để tránh lộ tài khoản tồn tại.
            if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Result<AuthenticatedUserDto>.Failure(Error.InvalidCreadential);
            }

            // Chỉ trả dữ liệu cần cho Controller tạo cookie.
            AuthenticatedUserDto dto = new()
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                RoleName = user.Role.RoleName.ToString()
            };

            return Result<AuthenticatedUserDto>.Success(dto);
        }

        public async Task<Result> RegisterAsync(RegisterRequestDto request)
        {
            // Kiểm tra lại DTO ở tầng nghiệp vụ.
            if (string.IsNullOrWhiteSpace(request.FullName)
                || string.IsNullOrWhiteSpace(request.Email)
                || string.IsNullOrWhiteSpace(request.Password))
            {
                return Result.Failure(Error.InvalidInput);
            }

            // Chuẩn hóa dữ liệu trước khi kiểm tra trùng.
            string fullName = request.FullName.Trim();
            string email = request.Email.Trim().ToLowerInvariant();

            // Email phải duy nhất.
            bool emailExists = await _dbContext.Users.AnyAsync(x => x.Email == email);
            if (emailExists)
            {
                return Result.Failure(Error.EmailAlreadyExist);
            }

            // Tài khoản tự đăng ký luôn nhận role USER.
            Role? userRole = await _dbContext.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.RoleName == UserRole.USER);

            if (userRole is null)
            {
                return Result.Failure(new Error("Auth.RoleMissing", "Role USER chưa được cấu hình."));
            }

            // Chỉ lưu password hash BCrypt.
            User user = new()
            {
                Email = email,
                FullName = fullName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                RoleId = userRole.Id
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }
    }
}
