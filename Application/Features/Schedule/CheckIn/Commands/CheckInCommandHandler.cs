using Application.Interfaces.Repositories;
using Domain;
using Domain.Entity;
using Domain.Enums;
using MediatR;

namespace Application.Features.Schedule.CheckIn.Commands
{
    public class CheckInCommandHandler : IRequestHandler<CheckInCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;

        private const int PreClassCheckInMinutes = 15;
        private const int PostClassCheckInMinutes = 10;

        public CheckInCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(CheckInCommand request, CancellationToken cancellationToken)
        {
            var booking = await _unitOfWork.Bookings.GetBookingDetailsAsync(request.BookingId)
                ?? throw new NotFoundException(nameof(Booking), request.BookingId);

            if (booking.UserId != request.UserId)
                throw new BusinessRuleException("Access denied.");

            if (booking.Status != BookingStatus.Confirmed.ToString())
                throw new BusinessRuleException($"Booking cannot be canceled (current status: {booking.Status}).");

            var bookingForSchedule = await _unitOfWork.Bookings.GetUserConfirmedBookingForScheduleAsync(request.UserId, booking.ClassScheduleId);

            if (bookingForSchedule == null || bookingForSchedule.UserId != request.UserId)
            {
                throw new NotFoundException(nameof(Booking), request.BookingId);
            }

            if (bookingForSchedule.Status == BookingStatus.CheckedIn.ToString())
            {
                throw new BusinessRuleException("You are already checked into this class.");
            }

            if (booking.Status != BookingStatus.Confirmed.ToString())
            {
                throw new BusinessRuleException($"Cannot check in. Booking status is '{bookingForSchedule.Status}'.");
            }

            var classStartTime = bookingForSchedule.ClassSchedule.StartTime;
            var currentTime = DateTime.UtcNow;

            var checkInWindowStart = classStartTime.AddMinutes(-PreClassCheckInMinutes);
            var checkInWindowEnd = classStartTime.AddMinutes(PostClassCheckInMinutes);

            if (currentTime < checkInWindowStart)
            {
                throw new BusinessRuleException($"Check-in is not yet open. You can check in starting at {checkInWindowStart.ToLocalTime()}.");
            }

            if (currentTime > checkInWindowEnd)
            {
                throw new BusinessRuleException($"Check-in window has closed. The class started at {classStartTime.ToLocalTime()}.");
            }

            bookingForSchedule.Status = BookingStatus.CheckedIn.ToString();
            bookingForSchedule.CheckedIn = true;

            _unitOfWork.Bookings.Update(booking);
            await _unitOfWork.CompleteAsync();

            return "Successfully checked in to the class.";
        }
    }
}
