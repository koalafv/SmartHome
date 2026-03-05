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
        public WeatherRecord CurrentWeather { get; set; }
        public List<WeatherRecord> WeatherHistory { get; set; } 
        public IndexModel ( ApplicationContext context )
        {
            _context = context;
        }

        public void OnGet ( )
        {
            CurrentSettings = _context.DeviceSettings.FirstOrDefault() ?? new DeviceSettings { Threshold = 30, PumpDuration = 2000, IsAutoMode = false };

            var cutoffTime = DateTime.Now.AddHours(-200);
            History = _context.SensorReadings
                .Where(s => s.ReadingDate >= cutoffTime)
                .OrderBy(s => s.ReadingDate)
                .ToList();
            History.Reverse();

            CurrentWeather = _context.WeatherRecords
                                .OrderByDescending(x => x.CheckedAt)
                                .FirstOrDefault();

            WeatherHistory = _context.WeatherRecords
                                .Where(x => x.CheckedAt >= cutoffTime)
                                .OrderBy(x => x.CheckedAt)
                                .ToList();

        }

        public async Task<IActionResult> OnPostLogoutAsync ( )
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Login");
        }
    }
}