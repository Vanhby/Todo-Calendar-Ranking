using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StreakHub.API.Models_Generated;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//dotnet ef dbcontext scaffold "Host=ep-tiny-thunder-aoe1d14b-pooler.c-2.ap-southeast-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_hm6vBdKC1cGX;SslMode=Require;Trust Server Certificate=true;" Npgsql.EntityFrameworkCore.PostgreSQL -o Models_Generated --force
//AUTH Service
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuerSigningKey = true,
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
//                .GetBytes(builder.Configuration.GetSection("Jwt:Key").Value!)),
//            ValidateIssuer = true,
//            ValidIssuer = builder.Configuration.GetSection("Jwt:Issuer").Value,
//            ValidateAudience = true,
//            ValidAudience = builder.Configuration.GetSection("Jwt:Audience").Value,
//            ValidateLifetime = true
//        };
//    });
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
