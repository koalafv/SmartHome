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
        public DeviceSettings CurrentSettings { get; set; }
        public List<SensorReading> History { get; set; } = new();

        public IndexModel ( ApplicationContext context )
        {
            _context = context;
        }

        public void OnGet ( )
        {
            CurrentSettings = _context.DeviceSettings.FirstOrDefault() ?? new DeviceSettings { Threshold = 30, PumpDuration = 2000, IsAutoMode = false };

            var cutoff = DateTime.Now.AddHours(-24);

            History = _context.SensorReadings
                .Where(s => s.ReadingDate >= cutoff)
                .OrderBy(s => s.ReadingDate)
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