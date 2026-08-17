using Microsoft.EntityFrameworkCore;
using SKDJK.Data;
using SKDJK.Dtos;
using SKDJK.Models.Commons;
using SKDJK.Services.Interfaces;
using SKDJK.Models.enums;
using SKDJK.Models;

namespace SKDJK.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext dbContext;
        public AuthService(ApplicationDbContext _dbContext)
        {
            this.dbContext = _dbContext;
        }
        public async Task<Result<AuthenticatedUserDto>> LoginAsync(string Email, string Password)
        {
           if(string.IsNullOrWhiteSpace(Email) ||
              string.IsNullOrWhiteSpace(Password))
           {
                return Result<AuthenticatedUserDto>.Failure(Error.InvalidInput);
           }

            Email = Email.Trim().ToLower();

            var user = await dbContext
                .Users
                .Include(u => u.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == Email);

            if (user == null)
                return Result<AuthenticatedUserDto>
                    .Failure(Error.InvalidCreadential);
            bool IsCorrectPassword = BCrypt.Net.BCrypt.Verify(Password, user.PasswordHash);

            if (!IsCorrectPassword)
                return Result<AuthenticatedUserDto>
                    .Failure(Error.InvalidCreadential);

            var authenticatedUser = new AuthenticatedUserDto
            {
                Email = Email,
                FullName = user.FullName,
                RoleName = user.Role.RoleName.ToString(),
                UserId = user.Id
            };

            return Result<AuthenticatedUserDto>
                .Success(authenticatedUser);
        }

        public async Task<Result> RegisterAsync(string FullName, string Email, string Password)
        {
            if(string.IsNullOrWhiteSpace(FullName) ||
               string.IsNullOrWhiteSpace(Email)    ||
               string.IsNullOrWhiteSpace(Password))
            {
                return Result
                    .Failure(Error.InvalidInput);
            }
            FullName = FullName.Trim();
            Email = Email.Trim().ToLower();

            bool emailExist = await dbContext
                .Users
                .AnyAsync(u => u.Email == Email);

            if (emailExist)
            {
                return Result.Failure(Error.EmailAlreadyExist);
            }


            var userRole = await dbContext
                .Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.RoleName == UserRole.USER);

            if (userRole == null)
            {
                throw new InvalidOperationException("User role chua duoc cau hinh trong he thong");
            }

            string PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password);

            var user = new User
            {
                Email = Email,
                FullName = FullName,
                PasswordHash = PasswordHash,
                RoleId = userRole.Id
            };

            await dbContext.AddAsync(user);
            await dbContext.SaveChangesAsync();

            return Result.Success();
        }
    }
}
