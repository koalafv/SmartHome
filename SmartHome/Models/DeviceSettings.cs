using System.ComponentModel.DataAnnotations;

namespace SmartHome.Models
{
    public class DeviceSettings
    {
        [Key]
        public int Id { get; set; }
        public int Threshold { get; set; }
        public int PumpDuration { get; set; }
        public bool IsAutoMode { get; set; }
    }
}
