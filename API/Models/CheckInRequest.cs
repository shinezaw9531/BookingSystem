using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class CheckInRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int BookingId { get; set; }
    }
}
