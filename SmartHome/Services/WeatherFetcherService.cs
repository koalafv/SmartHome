using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR; // <--- NOWOŚĆ: Do obsługi danych na żywo
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SmartHome.Models;
using System.Linq;
using Hubs; // <--- Upewnij się, że masz tu namespace swojego PotHub (może być SmartHome.Hubs)

namespace SmartHome.Services
{
    public class WeatherFetcherService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<PotHub> _hubContext; // <--- NOWOŚĆ
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "0b123547c8b74085a6472150260503";

        // DODALIŚMY IHubContext DO KONSTRUKTORA
        public WeatherFetcherService(IServiceScopeFactory scopeFactory, IHubContext<PotHub> hubContext)
        {
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
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

                Console.WriteLine($"[LOG] Następne pobranie pogody dla API punktualnie o: {nextRunTime:HH:mm:ss}");

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

                var now = DateTime.Now;
                int remainder = now.Minute % 15;

                int minutesToAddOrSubtract = remainder >= 8 ? (15 - remainder) : -remainder;

                DateTime perfectTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0)
                                        .AddMinutes(minutesToAddOrSubtract);

                var weather = new WeatherRecord
                {
                    City = root.GetProperty("location").GetProperty("name").GetString(),
                    Temperature = root.GetProperty("current").GetProperty("temp_c").GetDouble(),
                    WindSpeed = root.GetProperty("current").GetProperty("wind_kph").GetDouble(),
                    RainChance = (int)root.GetProperty("current").GetProperty("precip_mm").GetDouble(),
                    CheckedAt = perfectTime 
                };

                db.WeatherRecords.Add(weather);
                await db.SaveChangesAsync();


                string timeLabel = weather.CheckedAt.ToString("HH:mm");
                await _hubContext.Clients.All.SendAsync("UpdateWeather", weather.City, weather.Temperature, weather.WindSpeed, timeLabel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LOG] Błąd API: {ex.Message}");
            }
        }
    }
}