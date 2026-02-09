using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using UserAdminPanel.DAL.Interfaces;
using UserAdminPanel.DAL.Models;

namespace UserAdminPanel.DAL.Services
{
    public class UserService
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUsersRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IPasswordHasher passwordHasher, IUsersRepository userRepository, IJwtTokenService jwtTokenService, IHttpContextAccessor httpContextAccessor)
        {
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> Register(string email, string password)
        {
            var hashedPassword = _passwordHasher.Generate(password);
            string userId = Guid.NewGuid().ToString();

            var user = new ApplicationUser
            {
                Id = userId,
                Email = email,
                PasswordHash = hashedPassword,
                UserName = email // Assuming username is the same as email
            };

            await _userRepository.Add(user);
            return userId;
        }

        public async Task<ApplicationUser?> GetUserByEmail(string email)
        {
            var user = await _userRepository.GetByEmail(email);
            return user;
        }

        public async Task<ApplicationUser?> GetUserById(string? userId)
        {
            var user = await _userRepository.GetById(userId);
            return user;
        }

        public async Task<List<ApplicationUser>> GetAllUsers()
        {
            return (await _userRepository.GetAll())
                .ToList();
        }
                
        public async Task UpdateUser(ApplicationUser user)
        {
            await _userRepository.Update(user);
        }

        public async Task DeleteUser(string id)
        {
            await _userRepository.Delete(id);
        }

        public async Task SendConfirmationEmail(string email, string userId)
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";

            string verificationLink = $"{baseUrl}/Account/VerifyEmail?userId={userId}";

            var fromAddress = new MailAddress("ai.muratova@gmail.com", "UserAdminPanel");
            var toAddress = new MailAddress(email);
            const string subject = "Please confirm your account";
            string emailBody = $"Please confirm your account by <a href='{verificationLink}'>clicking here</a>.";

            var smtpClient = new SmtpClient("smtp.gmail.com") // Use your SMTP server
            {
                Port = 587,
                Credentials = new NetworkCredential("ai.muratova@gmail.com", "ttogavnjcxyydpoz"), // Use an App Password for services like Gmail
                EnableSsl = true,
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = emailBody,
                IsBodyHtml = true // Important for sending HTML links
            })
            {
                await smtpClient.SendMailAsync(message);
            }
        }
    }
}
