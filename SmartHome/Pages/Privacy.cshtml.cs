using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartHome.Models;

namespace SmartHome.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly Models.ApplicationContext _context;
        public List<User> Users { get; set; } = new List<User>();
        public PrivacyModel ( Models.ApplicationContext context )
        {
            _context = context;
        }

        public void OnGet ( )
        {
            Users = _context.Users.ToList();
        }
    }

}
