using Microsoft.Data.SqlClient;
using System.Net.Mail;
using System.Text.RegularExpressions;
using UserAdminPanel.DAL.Models;
using UsersAdminPanel.Models.ViewModels;

namespace UsersAdminPanel.Models.Services
{
    public class UserServiceMapper
    {
        private readonly UserAdminPanel.DAL.Services.UserService _userService;

        public UserServiceMapper(UserAdminPanel.DAL.Services.UserService userService)
        {
            _userService = userService;
        }

        public async Task<ResultModel> Register(string email, string password, string confirmPassword)
        {
            var result = new ResultModel();
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                //validation of email and password can be added here if needed
                Regex regex = new Regex(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$", RegexOptions.IgnoreCase);
                if (!regex.IsMatch(email))
                {
                    result.Success = false;
                    result.Message = "Invalid email format";
                    return result;
                }

                if(password.Length < 1)
                {
                    result.Success = false;
                    result.Message = "Password must be at least 1 characters long";
                    return result;
                }

                if (password != confirmPassword)
                {
                    result.Success = false;
                    result.Message = "Confirm password mismatch";
                    return result;
                }

                try
                {
                    await _userService.Register(email, password);
                    result.Success = true;
                }
                catch (SqlException exc)
                {
                    result.Success = false;
                    result.Message = $"Failed to register user in database: this login is already taken";
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = $"Failed to register user: {ex.Message}";
                }
            }
            else
            {
                result.Success = false;
                result.Message = "Failed to register user. Email or password was not specified";
            }
                        
            return result;
        }

        public async Task<LoginResultModel> Login(string email, string password)
        {
            var result = new LoginResultModel();
            if (!string.IsNullOrEmpty(email))
            {
                try
                {
                    var token = await _userService.Login(email, password);
                    result.Success = true;
                    result.Token = token;
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = $"Failed to login user: {ex.Message}";
                }
            }
            else
            {
                result.Success = false;
                result.Message = "Failed to register user";
            }

            return result;
        }

        public async Task<LoginResultModel> GetUserById(string userId)
        {
            var result = new LoginResultModel();    

            var user = await _userService.GetUserById(userId);

            if (user == null || String.IsNullOrEmpty(user.Id)) {
                result.Success = false;
                result.Message = "User not found";
            }
            else if(user.LockoutEnabled)
            {
                result.Success = false;
                result.Message = "User is blocked";
            }
            else
            {
                result.Success = true;
                result.User = new ApplicationUser() { Email = user?.Email ?? "" };
            }
            return result;
        }

        public async Task<List<UserViewModel>> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            var result = new List<UserViewModel>();

            foreach (var item in users)
            {
                var newItem = new UserViewModel()
                {
                    Id = item.Id,
                    Email = item.Email,
                    Name = item.UserName,
                    Status = !item.LockoutEnabled ? "Active" : "Blocked",
                    Confirmed = item.EmailConfirmed ? "Confirmed" : "Not confirmed",
                    LastSeen = item.LastEnteredDate
                };
                result.Add(newItem);
            }

            return result;
        }

        public async Task<ResultModel> BlockUser(List<string> usersList)
        {
            var result = new ResultModel();
            var errors = new List<string>();

            if(usersList == null || usersList.Count == 0)
            {
                result.Success = false;
                result.Message = "No users selected for blocking";
                return result;
            }

            foreach (var userId in usersList)
            {
                var user = await _userService.GetUserById(userId);

                if (user == null)
                {
                    errors.Add($"User info not found");
                }
                else if (user.LockoutEnabled)
                {
                    errors.Add($"User {user.Email} is already blocked");
                }
                else
                {
                    user.LockoutEnabled = true;
                    await _userService.UpdateUser(user);
                }
            }

            if (errors.Count > 0)
            {
                result.Success = false;
                result.Message = "Some users were not blocked successfully";
                result.Errors = errors;
            }
            else
            {
                result.Success = true;
                result.Message = "All users blocked successfully";
            }

            return result;
        }

        public async Task<ResultModel> UnblockUser(List<string> usersList)
        {
            var result = new ResultModel();
            var errors = new List<string>();

            if (usersList == null || usersList.Count == 0)
            {
                result.Success = false;
                result.Message = "No users selected for unblocking";
                return result;
            }

            foreach (var userId in usersList)
            {
                var user = await _userService.GetUserById(userId);

                if (user == null)
                {
                    errors.Add($"User info not found");
                }
                else if (!user.LockoutEnabled)
                {
                    errors.Add($"User {user.Email} is already unblocked");
                }
                else
                {
                    user.LockoutEnabled = false;
                    await _userService.UpdateUser(user);
                }
            }

            if (errors.Count > 0)
            {
                result.Success = false;
                result.Message = "Some users were not unblocked successfully";
                result.Errors = errors;
            }
            else
            {
                result.Success = true;
                result.Message = "All users unblocked successfully";
            }

            return result;
        }

        public async Task<ResultModel> DeleteUser(List<string> idList)
        {
            var result = new ResultModel();
            var errors = new List<string>();

            if(idList == null || idList.Count == 0)
            {
                result.Success = false;
                result.Message = "No users selected for deletion";
                return result;
            }

            foreach (var userId in idList)
            {
                try
                {
                    await _userService.DeleteUser(userId);
                }
                catch (Exception ex)
                {
                    errors.Add($"Failed to delete user with ID {userId}: {ex.Message}");
                }
            }

            if (errors.Count > 0)
            {
                result.Success = false;
                result.Message = "Some users were not deleted successfully";
                result.Errors = errors;
            }
            else
            {
                result.Success = true;
            }

            return result;
        }

        public async Task<ResultModel> DeleteUnconfirmedUser()
        {
            var result = new ResultModel();

            try
            {                
                var unconfirmedUsers = (await _userService.GetAllUsers())
                    .Where(u => !u.EmailConfirmed).ToList();
             
                foreach (var user in unconfirmedUsers)
                {                    
                    await _userService.DeleteUser(user.Id);
                }
                
                result.Success = true;
                result.Message = "Unconfirmed users deleted successfully";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Failed to delete unconfirmed users: {ex.Message}";
            }
            return result;
        }
    }
}
