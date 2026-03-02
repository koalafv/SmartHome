using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHome.Models
{
    [Table("Devices")]
    public class Device
    {
        [Key]
        public int DeviceId { get; set; }

        [Required]
        [MaxLength(50)]
        public string MacAddress { get; set; } = string.Empty;

        [MaxLength(100)]
        public string CustomName { get; set; } = "Moje nowe urządzenie"; // Tekst po polsku

        [Required]
        public int DeviceTypeId { get; set; }
        [ForeignKey("DeviceTypeId")]
        public virtual DeviceType Type { get; set; } = null!;

        public int? OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public virtual User? Owner { get; set; }
    }
}