using Application.Interfaces.External;
using Application.Interfaces.Repositories;
using Domain;
using Domain.Entity;
using Domain.Enums;
using Hangfire;
using MediatR;

namespace Application.Features.Schedule.CancelBooking.Commands
{
    public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IScheduledBookingService _scheduledBookingService;

        public CancelBookingCommandHandler(IUnitOfWork unitOfWork, IScheduledBookingService scheduledBookingService)
        {
            _unitOfWork = unitOfWork;
            _scheduledBookingService = scheduledBookingService;
        }

        public async Task<string> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await _unitOfWork.Bookings.GetBookingDetailsAsync(request.BookingId)
                ?? throw new NotFoundException(nameof(Booking), request.BookingId);

            if (booking.UserId != request.UserId)
                throw new BusinessRuleException("Access denied.");

            if (booking.Status != BookingStatus.Confirmed.ToString())
                throw new BusinessRuleException($"Booking cannot be canceled (current status: {booking.Status}).");

            var schedule = await _unitOfWork.ClassSchedules.GetByIdAsync(booking.ClassScheduleId)
                ?? throw new NotFoundException(nameof(ClassSchedule), booking.ClassScheduleId);

            TimeSpan timeUntilStart = schedule.StartTime - DateTime.UtcNow;

            booking.Status = BookingStatus.Canceled.ToString();
            _unitOfWork.Bookings.Update(booking);

            if (timeUntilStart.TotalHours >= 4)
            {
                var userPackage = await _unitOfWork.UserPackages.GetByIdAsync(booking.UserPackageId!.Value);
                if (userPackage != null)
                {
                    userPackage.RemainingCredits += booking.CreditsDeducted;
                    _unitOfWork.UserPackages.Update(userPackage);
                }
            }

            await _unitOfWork.CompleteAsync();

            BackgroundJob.Enqueue(
                () => _scheduledBookingService.ProcessWaitlistAfterCancellation(schedule.Id)
            );

            return $"Booking {request.BookingId} canceled successfully. Refund status: {(timeUntilStart.TotalHours >= 4 ? "Credits returned." : "No refund (within 4-hour window).")}";
        }
    }
}
