using System.ComponentModel.DataAnnotations;
using System;

namespace SmartHome.Models
{
    public class WeatherRecord
    {
        [Key]
        public int Id { get; set; }
        public string City { get; set; }
        public double Temperature { get; set; }
        public double WindSpeed { get; set; }
        public int RainChance { get; set; }
        public DateTime CheckedAt { get; set; }
        public double Pressure { get; set; }
        public int Humidity { get; set; }
    }
}