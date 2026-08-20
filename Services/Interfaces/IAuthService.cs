using SKDJK.Dtos;
using SKDJK.Models.commons;

namespace SKDJK.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Result> RegisterAsync(RegisterRequestDto request);
        Task<Result<AuthenticatedUserDto>> LoginAsync(LoginRequestDto request);
    }
}
