using Microsoft.AspNetCore.SignalR;
using SmartHome.Models;
using System;
using System.Linq; // Wymagane do FirstOrDefault()
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

        // 1. ZAPIS POMIARÓW DO BAZY (Twój kod - bez zmian)
        public async Task SendStatusFromDevice ( int soilMoisture, int tankLevel, float temperature, float pressure )
        {
            Console.WriteLine($"[SignalR] Status z ESP: Gleba={soilMoisture}%, Woda={tankLevel}, Temp={temperature}C, Cisn={pressure}hPa");

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

            // Rozsyłamy wartości na ekran (Live Dashboard)
            await Clients.All.SendAsync("UpdateStatus", soilMoisture, tankLevel, temperature, pressure);
        }

        // 2. NOWOŚĆ: ZAPIS USTAWIEŃ DO BAZY I WYSYŁKA DO ESP
        public async Task SendSettingsToDevice ( int threshold, bool isAutoMode, int pumpDuration )
        {
            Console.WriteLine($"[LOG {DateTime.Now:HH:mm:ss}] Hub: Zapisywanie ustawien do bazy i wysylanie do ESP...");

            try
            {
                // Szukamy, czy mamy już wpis z ustawieniami
                var settings = _context.DeviceSettings.FirstOrDefault();
                if (settings == null)
                {
                    settings = new DeviceSettings();
                    _context.DeviceSettings.Add(settings);
                }

                // Aktualizujemy dane
                settings.Threshold = threshold;
                settings.IsAutoMode = isAutoMode;
                settings.PumpDuration = pumpDuration;

                await _context.SaveChangesAsync();
                Console.WriteLine("[DB] Pomyślnie zaktualizowano ustawienia w bazie SQL.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB ERROR] Błąd zapisu ustawień do bazy: {ex.Message}");
            }

            // Rozsyłamy do wszystkich urządzeń (w tym do ESP)
            await Clients.All.SendAsync("ReceiveSettings", threshold, isAutoMode, pumpDuration);
        }

        // 3. NOWOŚĆ: POBIERANIE USTAWIEŃ PRZEZ ESP PRZY STARCIE
        public async Task GetCurrentSettings ( )
        {
            Console.WriteLine($"[LOG {DateTime.Now:HH:mm:ss}] Hub: Urzadzenie {Context.ConnectionId} prosi o aktualne ustawienia.");

            var settings = _context.DeviceSettings.FirstOrDefault();

            if (settings != null)
            {
                // Zwracamy z bazy tylko do urządzenia, które zapytało (ESP)
                await Clients.Caller.SendAsync("ReceiveSettings", settings.Threshold, settings.IsAutoMode, settings.PumpDuration);
            }
            else
            {
                // Domyślne wartości, jeśli baza jest jeszcze pusta
                await Clients.Caller.SendAsync("ReceiveSettings", 30, false, 2000);
            }
        }

        // 4. WYMUSZENIE PODLEWANIA
        public async Task ForceWatering ( )
        {
            Console.WriteLine($"[LOG {DateTime.Now:HH:mm:ss}] Hub: Wymuszenie podlewania!");
            await Clients.Others.SendAsync("ForceWatering");
        }
    }
}