using SKDJK.Models;
using SKDJK.Dtos;
using SKDJK.Models.commons;

namespace SKDJK.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Result> RegisterAsync(string FullName, string Email, string Password);
        Task<Result<AuthenticatedUserDto>> LoginAsync(string Email, string Password);
    }
}
