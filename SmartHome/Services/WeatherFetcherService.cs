using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SmartHome.Models;
using System.Linq;

namespace SmartHome.Services
{
    public class WeatherFetcherService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly HttpClient _httpClient;
        // Twój klucz API podpięty na sztywno
        private readonly string _apiKey = "0b123547c8b74085a6472150260503";

        public WeatherFetcherService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _httpClient = new HttpClient();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await FetchWeatherAsync();

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                int minutesToAdd = 15 - (now.Minute % 15);
                var nextRunTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0).AddMinutes(minutesToAdd);
                var delay = nextRunTime - now;

                await Task.Delay(delay, stoppingToken);

                if (!stoppingToken.IsCancellationRequested)
                {
                    await FetchWeatherAsync();
                }
            }
        }

        private async Task FetchWeatherAsync()
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

                var settings = db.DeviceSettings.FirstOrDefault();
                string cityToFetch = settings?.City;

                if (string.IsNullOrWhiteSpace(cityToFetch))
                {
                    cityToFetch = "Bytom"; 
                }

                string safeCity = Uri.EscapeDataString(cityToFetch);
                string url = $"http://api.weatherapi.com/v1/current.json?key={_apiKey}&q={safeCity}&aqi=no";

                var response = await _httpClient.GetStringAsync(url);

                using JsonDocument doc = JsonDocument.Parse(response);
                var root = doc.RootElement;

                var weather = new WeatherRecord
                {
                    City = root.GetProperty("location").GetProperty("name").GetString(),
                    Temperature = root.GetProperty("current").GetProperty("temp_c").GetDouble(),
                    WindSpeed = root.GetProperty("current").GetProperty("wind_kph").GetDouble(),
                    // Jako szansę na deszcz możemy wziąć opad w mm z Twojego JSON-a (precip_mm)
                    RainChance = (int)root.GetProperty("current").GetProperty("precip_mm").GetDouble(),
                    CheckedAt = DateTime.Now
                };

                db.WeatherRecords.Add(weather);
                await db.SaveChangesAsync();

                Console.WriteLine($"[LOG {DateTime.Now:HH:mm:ss}] Zapisano pogodę w tle: {weather.City} ({weather.Temperature}°C)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LOG] Błąd API: {ex.Message}");
            }
        }
    }
}