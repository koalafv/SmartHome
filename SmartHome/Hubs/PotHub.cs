using Microsoft.AspNetCore.SignalR;
using SmartHome.Models; // Upewnij się, że to nazwa Twojego projektu/modeli
using System;
using System.Threading.Tasks;

namespace Hubs
{
    public class PotHub : Hub
    {
        private readonly ApplicationContext _context;

        public PotHub ( ApplicationContext context )
        {
            _context = context;
        }

        public override async Task OnConnectedAsync ( )
        {
            Console.WriteLine($"[SignalR] Nowe urządzenie: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        // [LOG] 3. Odbiór danych z ESP32
        public async Task SendStatusFromDevice ( int soilMoisture, int tankLevel, float temperature, float pressure )
        {
            Console.WriteLine($"[SignalR] Status z ESP: Gleba={soilMoisture}%, Woda={tankLevel}, Temp={temperature}C, Cisn={pressure}hPa");

            // --- SEKCJA ZAPISU DO BAZY DANYCH ---
            try
            {
                var reading = new SensorReading
                {
                    ReadingDate = DateTime.Now,
                    SoilMoisture = soilMoisture,
                    TankLevel = tankLevel,
                    Temperature = temperature,
                    Pressure = pressure
                };

                _context.SensorReadings.Add(reading);
                await _context.SaveChangesAsync();
                Console.WriteLine("[DB] Pomyślnie zapisano pomiar w bazie SQL.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB ERROR] Błąd zapisu do bazy: {ex.Message}");
            }
            // ------------------------------------

            // Rozsyłamy wartości na ekran (Live Dashboard)
            await Clients.All.SendAsync("UpdateStatus", soilMoisture, tankLevel, temperature, pressure);
        }

        public async Task SendSettingsToDevice ( int threshold, bool isAutoMode, int pumpDuration )
        {
            Console.WriteLine($"[LOG {DateTime.Now:HH:mm:ss}] Hub: Wysylanie ustawien do ESP...");
            await Clients.Others.SendAsync("ReceiveSettings", threshold, isAutoMode, pumpDuration);
        }

        public async Task ForceWatering ( )
        {
            Console.WriteLine($"[LOG {DateTime.Now:HH:mm:ss}] Hub: Wymuszenie podlewania!");
            await Clients.Others.SendAsync("ForceWatering");
        }
    }
}