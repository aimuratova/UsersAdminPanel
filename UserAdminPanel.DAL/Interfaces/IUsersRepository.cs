using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAdminPanel.DAL.Models;

namespace UserAdminPanel.DAL.Interfaces
{
    public interface IUsersRepository
    {
        Task Add(ApplicationUser user);
        Task<ApplicationUser?> GetByEmail(string email);
        Task<ApplicationUser?> GetById(string? userId);
        Task<List<ApplicationUser>> GetAll();
        Task Update(ApplicationUser user);
        Task Delete(string id);
    }
}
