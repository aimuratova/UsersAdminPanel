using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserAdminPanel.DAL.Models
{
    public class ApplicationUser
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }  // Store hashed password in the database        
        public bool LockoutEnabled { get; set; }
        public string UserName { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public DateTime? LastEnteredDate { get; set; }
    }
}
