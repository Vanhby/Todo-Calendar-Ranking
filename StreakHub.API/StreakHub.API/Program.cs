using Microsoft.EntityFrameworkCore;
using StreakHub.API.Data;
using StreakHub.API.Interfaces;
using StreakHub.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 2. Kết nối Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

builder.Services.AddScoped<IDndService, DndService>();
builder.Services.AddScoped<IReminderService, ReminderService>();
builder.Services.AddHostedService<EmailReminderWorker>(); 
builder.Services.AddScoped<IStreakService, StreakService>();
builder.Services.AddScoped<IShareService, ShareService>();
builder.Services.AddScoped<ITodoService, TodoService>();

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// 4. Kích hoạt CORS (Gọi 1 lần sau khi Build app)
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();

app.Run();