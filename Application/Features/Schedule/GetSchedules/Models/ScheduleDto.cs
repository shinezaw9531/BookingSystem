namespace Application.Features.Schedule.GetSchedules.Models
{
    public class ScheduleDto
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
        public string InstructorName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int AvailableSlots { get; set; }
        public int Capacity { get; set; }
        public bool IsBookedByUser { get; set; }
        public bool IsOnWaitlist { get; set; }
    }
}
