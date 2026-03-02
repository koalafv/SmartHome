using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHome.Models
{
    [Table("SensorReadings")]
    public class SensorReading
    {
        [Key]
        public int Id { get; set; }

        public DateTime ReadingDate { get; set; } = DateTime.Now;

        public int SoilMoisture { get; set; }
        public int TankLevel { get; set; }
        public float Temperature { get; set; }
        public float Pressure { get; set; }
    }
}