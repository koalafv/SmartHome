using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartHome.Models;

namespace SmartHome.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationContext _context;

        public List<SensorReading> History { get; set; } = new();

        public IndexModel ( ApplicationContext context )
        {
            _context = context;
        }

        public void OnGet ( )
        {
            History = _context.SensorReadings
                              .OrderByDescending(s => s.ReadingDate)
                              .Take(10)
                              .ToList();
            History.Reverse();
        }

        public async Task<IActionResult> OnPostLogoutAsync ( )
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Login");
        }
    }
}