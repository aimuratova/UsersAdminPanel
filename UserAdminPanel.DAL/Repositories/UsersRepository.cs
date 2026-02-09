using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAdminPanel.DAL.Interfaces;
using UserAdminPanel.DAL.Models;

namespace UserAdminPanel.DAL.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public UsersRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection2");
        }

        public async Task Add(ApplicationUser user)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("[dbo].[spInsertUser]", connection))
                {
                    // Set the command type to StoredProcedure
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters
                    command.Parameters.Add(new SqlParameter("@Id", user.Id));
                    command.Parameters.Add(new SqlParameter("@Email", user.Email));
                    command.Parameters.Add(new SqlParameter("@EmailConfirmed", false));
                    command.Parameters.Add(new SqlParameter("@PasswordHash", user.PasswordHash));
                    command.Parameters.Add(new SqlParameter("@LockoutEnabled", false));
                    command.Parameters.Add(new SqlParameter("@UserName", user.UserName));
                    command.Parameters.Add(new SqlParameter("@LastEnteredDate", DateTime.Now));

                    // Open the connection asynchronously
                    await connection.OpenAsync();

                    // Execute the stored procedure asynchronously
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task Delete(string id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("[dbo].[spDeleteUser]", connection))
                {
                    // Set the command type to StoredProcedure
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters
                    command.Parameters.Add(new SqlParameter("@Id", id));

                    // Open the connection asynchronously
                    await connection.OpenAsync();

                    // Execute the stored procedure asynchronously
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<ApplicationUser>> GetAll()
        {
            var listUsers = new List<ApplicationUser>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("[dbo].[spGetAll]", connection))
                {
                    // Set the command type to StoredProcedure
                    command.CommandType = CommandType.StoredProcedure;

                    await connection.OpenAsync(); // Asynchronous connection open

                    // Execute the command asynchronously and get the reader
                    using var reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync()) // Asynchronous reading of rows
                    {
                        var user = new ApplicationUser
                        {
                            Id = reader.GetString(reader.GetOrdinal("Id")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            EmailConfirmed = reader.GetBoolean(reader.GetOrdinal("EmailConfirmed")),
                            LockoutEnabled = reader.GetBoolean(reader.GetOrdinal("LockoutEnabled")),
                            UserName = reader.GetString(reader.GetOrdinal("UserName")),
                            LockoutEnd = reader.IsDBNull(reader.GetOrdinal("LockoutEnd")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("LockoutEnd")),
                            LastEnteredDate = reader.IsDBNull(reader.GetOrdinal("LastEnteredDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("LastEnteredDate"))
                        };
                        listUsers.Add(user);
                    }
                }
            }
            return listUsers;
        }

        public async Task<ApplicationUser?> GetByEmail(string email)
        {
            var user = new ApplicationUser();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("[dbo].[spGetByEmail]", connection))
                {
                    // Set the command type to StoredProcedure
                    command.CommandType = CommandType.StoredProcedure;
                    // Add parameters
                    command.Parameters.Add(new SqlParameter("@Email", email));

                    await connection.OpenAsync(); // Asynchronous connection open

                    // Execute the command asynchronously and get the reader
                    using var reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync()) // Asynchronous reading of rows
                    {
                        user = new ApplicationUser
                        {
                            Id = reader.GetString(reader.GetOrdinal("Id")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            EmailConfirmed = reader.GetBoolean(reader.GetOrdinal("EmailConfirmed")),
                            PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                            LockoutEnabled = reader.GetBoolean(reader.GetOrdinal("LockoutEnabled")),
                            UserName = reader.GetString(reader.GetOrdinal("UserName")),
                            LockoutEnd = reader.IsDBNull(reader.GetOrdinal("LockoutEnd")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("LockoutEnd")),
                            LastEnteredDate = reader.IsDBNull(reader.GetOrdinal("LastEnteredDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("LastEnteredDate"))
                        };
                    }
                }
            }
            return user;
        }

        public async Task<ApplicationUser?> GetById(string? userId)
        {
            var user = new ApplicationUser();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("[dbo].[spGetById]", connection))
                {
                    // Set the command type to StoredProcedure
                    command.CommandType = CommandType.StoredProcedure;
                    // Add parameters
                    command.Parameters.Add(new SqlParameter("@Id", userId));

                    await connection.OpenAsync(); // Asynchronous connection open

                    // Execute the command asynchronously and get the reader
                    using var reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync()) // Asynchronous reading of rows
                    {
                        user = new ApplicationUser
                        {
                            Id = reader.GetString(reader.GetOrdinal("Id")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            EmailConfirmed = reader.GetBoolean(reader.GetOrdinal("EmailConfirmed")),
                            LockoutEnabled = reader.GetBoolean(reader.GetOrdinal("LockoutEnabled")),
                            UserName = reader.GetString(reader.GetOrdinal("UserName")),
                            LockoutEnd = reader.IsDBNull(reader.GetOrdinal("LockoutEnd")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("LockoutEnd")),
                            LastEnteredDate = reader.IsDBNull(reader.GetOrdinal("LastEnteredDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("LastEnteredDate"))
                        };
                    }
                }
            }
            return user;
        }

        public async Task Update(ApplicationUser user)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("[dbo].[spUpdateUser]", connection))
                {
                    // Set the command type to StoredProcedure
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters
                    command.Parameters.Add(new SqlParameter("@Id", user.Id));
                    command.Parameters.Add(new SqlParameter("@EmailConfirmed", user.EmailConfirmed));
                    command.Parameters.Add(new SqlParameter("@LockoutEnabled", user.LockoutEnabled));
                    command.Parameters.Add(new SqlParameter("@UserName", user.UserName));
                    command.Parameters.Add(new SqlParameter("@LastEnteredDate", user.LastEnteredDate));

                    // Open the connection asynchronously
                    await connection.OpenAsync();

                    // Execute the stored procedure asynchronously
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
