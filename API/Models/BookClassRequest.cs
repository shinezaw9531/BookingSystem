using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class BookClassRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int ClassScheduleId { get; set; }
    }
}
