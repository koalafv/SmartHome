using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartHome.Models;

namespace SmartHome.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly DoniczkaContext _context;
        public List<User> Users { get; set; } = new List<User>();
        public PrivacyModel ( DoniczkaContext context )
        {
            _context = context;
        }

        public void OnGet ( )
        {
            Users = _context.Users.ToList();
        }
    }

}
