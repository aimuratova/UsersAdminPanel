using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserAdminPanel.DAL.Interfaces;
using UserAdminPanel.DAL.Repositories;
using UserAdminPanel.DAL.Services;
using UsersAdminPanel.Models;
using UsersAdminPanel.Models.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddScoped<IUsersRepository, UsersRepository>();
services.AddScoped<IPasswordHasher, PasswordHasher>();
services.AddScoped<IJwtTokenService, JwtTokenService>();
services.AddScoped<UserService>();
services.AddScoped<UserServiceMapper>();


// Добавление сервисов аутентификации
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
            };
        });

// Добавление сервисов контроллеров с представлениями
services.AddControllersWithViews();

var app = builder.Build();

// Конфигурация конвейера HTTP-запросов
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
