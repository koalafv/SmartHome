using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHome.Models
{
    [Table("DeviceTypes")]
    public class DeviceType
    {
        [Key]
        public int TypeId { get; set; }

        [Required]
        [MaxLength(100)]
        public string TypeName { get; set; } = string.Empty; // Tutaj wpiszesz np. "Doniczka"

        // Relacja: Jeden typ może mieć wiele przypisanych fizycznych urządzeń
        public virtual ICollection<Device> Devices { get; set; } = new List<Device>();
    }
}