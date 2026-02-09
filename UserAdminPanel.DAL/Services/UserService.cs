using System;
using System.Collections.Generic;
using System.Linq;
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

        public UserService(IPasswordHasher passwordHasher, IUsersRepository userRepository, IJwtTokenService jwtTokenService)
        {
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
        }

        public async Task Register(string email, string password)
        {
            var hashedPassword = _passwordHasher.Generate(password);

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = email,
                PasswordHash = hashedPassword,
                UserName = email // Assuming username is the same as email
            };

            await _userRepository.Add(user);
        }

        public async Task<string> Login(string email, string password)
        {
            var user = await _userRepository.GetByEmail(email);
            if(user == null || String.IsNullOrEmpty(user.Id))
            {
                throw new Exception("Failed to login, user not found");
            }

            var result = _passwordHasher.Verify(password, user!.PasswordHash);

            if (!result)
            {
                throw new Exception("Failed to login, wrong password");
            }

            var token = _jwtTokenService.GenerateToken(user);
            return token;
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
    }
}
