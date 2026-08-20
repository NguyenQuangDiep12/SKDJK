using System.Security.Claims;

namespace SKDJK.Controllers
{
    // Giữ việc đọc UserId từ claim nhất quán và không nhận UserId từ form.
    internal static class ControllerClaimsExtensions
    {
        public static bool TryGetUserId(this ClaimsPrincipal user, out int userId)
        {
            return int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out userId) && userId > 0;
        }
    }
}
