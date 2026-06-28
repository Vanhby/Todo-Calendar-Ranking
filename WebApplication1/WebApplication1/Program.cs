using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StreakHub.API.Models_Generated;
using StreakHub.API.Service;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<Auth_Service>();

//dotnet ef dbcontext scaffold "Host=ep-tiny-thunder-aoe1d14b-pooler.c-2.ap-southeast-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_hm6vBdKC1cGX;SslMode=Require;Trust Server Certificate=true;" Npgsql.EntityFrameworkCore.PostgreSQL -o Models_Generated --force

//

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
//
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

  
}

app.UseDefaultFiles(); // Tự động tìm chạy file index.html khi vào web
app.UseStaticFiles();  // Cho phép đọc các file html, css, js trong thư mục wwwroot

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
