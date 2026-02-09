namespace UsersAdminPanel.Models.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string Confirmed { get; set; }
        public DateTime? LastSeen { get; set; }
    }
}
