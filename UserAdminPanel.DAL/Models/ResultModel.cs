using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserAdminPanel.DAL.Models
{
    public class ResultModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }

    public class LoginResultModel : ResultModel
    {
        public string Token { get; set; }
        public ApplicationUser User { get; set; }
    }
}
