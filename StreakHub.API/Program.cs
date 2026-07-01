using Microsoft.EntityFrameworkCore;
using StreakHub.API.Data;
using StreakHub.API.Interfaces;
using StreakHub.API.Service;
using StreakHub.API.Services;

var builder = WebApplication.CreateBuilder(args);

// 2. Kết nối Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cấp phát bộ nhớ đệm cho Session (nếu bạn vẫn muốn dùng) hoặc comment lại nếu bỏ hẳn
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(24);
    options.Cookie.HttpOnly = true;
});

builder.Services.AddControllers();
// Khai báo các Services
builder.Services.AddScoped<Auth_Service>();
builder.Services.AddScoped<IDndService, DndService>();
builder.Services.AddScoped<IReminderService, ReminderService>();
builder.Services.AddHostedService<EmailReminderWorker>();
builder.Services.AddScoped<IStreakService, StreakService>();
builder.Services.AddScoped<IShareService, ShareService>();

builder.Services.AddScoped<IRankingService, RankingService>();// CHỖ SỬA QUAN TRỌNG: Đăng ký TodoService của bạn để Controller gọi được qua Interface
builder.Services.AddScoped<ITodoService, TodoService>();

builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 4. Kích hoạt Middleware
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();
//app.UseHttpsRedirection();

app.UseSession();

// Cần để 2 dòng này trước Authorization và MapControllers để phục vụ file tĩnh trong wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

//app.UseHttpsRedirection();
app.UseAuthorization();

///////////////////////
app.UseStaticFiles();
///////////////////////

app.MapControllers();

app.Run();