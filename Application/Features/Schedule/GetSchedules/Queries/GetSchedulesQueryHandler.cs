using Application.Features.Schedule.GetSchedules.Models;
using Application.Interfaces.Repositories;
using Domain;
using Domain.Enums;
using MediatR;

namespace Application.Features.Schedule.GetSchedules.Queries
{
    public class GetSchedulesQueryHandler : IRequestHandler<GetSchedulesQuery, IReadOnlyList<ScheduleDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSchedulesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<ScheduleDto>> Handle(GetSchedulesQuery request, CancellationToken cancellationToken)
        {
            var country = await _unitOfWork.Countries.GetByCodeAsync(request.CountryCode);

            if (country == null)
            {
                throw new NotFoundException("Country", request.CountryCode);
            }
            var schedules = await _unitOfWork.ClassSchedules.GetSchedulesByCountryAsync(country.Id);

            if (schedules == null || !schedules.Any())
            {
                return new List<ScheduleDto>();
            }

            var userBookings = await _unitOfWork.Bookings.GetUserBookingsForSchedulesAsync(schedules.Select(s => s.Id).ToList());

            var results = new List<ScheduleDto>();

            foreach (var schedule in schedules)
            {
                int bookedCount = schedule.Bookings.Count(b => b.Status != BookingStatus.Canceled.ToString());
                int availableSlots = schedule.MaxCapacity - bookedCount;

                var userBooking = userBookings.FirstOrDefault(b => b.ClassScheduleId == schedule.Id);

                bool isBookedByUser = userBooking != null &&
                                      userBooking.Status == BookingStatus.Confirmed.ToString() ||
                                      userBooking.Status == BookingStatus.CheckedIn.ToString();

                bool isOnWaitlist = userBooking != null &&
                                    userBooking.Status == BookingStatus.Waitlisted.ToString();

                results.Add(new ScheduleDto
                {
                    Id = schedule.Id,
                    ClassName = schedule.Name,
                    InstructorName = "TBD",
                    StartTime = schedule.StartTime,
                    EndTime = schedule.EndTime,
                    Capacity = schedule.MaxCapacity,
                    AvailableSlots = Math.Max(0, availableSlots),
                    IsBookedByUser = isBookedByUser,
                    IsOnWaitlist = isOnWaitlist
                });
            }

            return results;
        }
    }
}
