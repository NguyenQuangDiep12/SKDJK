namespace SKDJK.Dtos;

// DTO nhận dữ liệu đăng ký từ Controller sau khi RegisterViewModel hợp lệ.
public sealed class RegisterRequestDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

// DTO nhận dữ liệu đăng nhập từ Controller sau khi LoginViewModel hợp lệ.
public sealed class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

// DTO thông tin đăng nhập thành công trả từ Service về Controller.
public sealed class AuthenticatedUserDto
{
    public int UserId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string RoleName { get; init; } = string.Empty;
}
