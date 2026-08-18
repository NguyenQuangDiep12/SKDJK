using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SKDJK.Data;
using SKDJK.Services;
using SKDJK.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
));

#region Cau hinh xac thuc voi Cookie Authentication
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Trang đăng nhập khi user chưa xác thực
        options.LoginPath = "/Account/Login";
        // Trang báo không có quyền
        options.AccessDeniedPath = "/Account/Forbidden";
        // ?returnUrl=/Lesson/MyLesson
        options.ReturnUrlParameter = "returnUrl";
        // Thời gian Authentication Ticket tồn tại
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        // User hoạt động thì có thể được gia hạn
        options.SlidingExpiration = true;
        // ============================
        // Authentication Cookie
        // ============================
        options.Cookie.Name = ".LearningSystem.Auth";
        // JavaScript không đọc được
        options.Cookie.HttpOnly = true;
        // Chỉ gửi cookie qua HTTPS
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        // Phù hợp với MVC authentication thông thường
        options.Cookie.SameSite = SameSiteMode.Lax;
        // Cookie có hiệu lực toàn website
        options.Cookie.Path = "/";
        // Authentication cookie là cookie thiết yếu
        options.Cookie.IsEssential = true;
    });

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".LearningSystem.Session";

    // Session het han neu khong duoc su dung trong 20 phut
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthorization();
#endregion


#region DI
builder.Services.Configure<CloudinaryOption>(builder.Configuration.GetSection("Cloudinary"));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<ILessonService, LessonService>();
builder.Services.AddHttpContextAccessor();
#endregion

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
