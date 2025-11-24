using Application.Interfaces.External;
using Application.Interfaces.Repositories;
using Domain.Entity;
using Domain.Enums;
using System.Threading;

namespace Infrastructure.Services
{
    public class ScheduledBookingService : IScheduledBookingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ScheduledBookingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ProcessWaitlistAfterCancellation(int classScheduleId)
        {
            var schedule = await _unitOfWork.ClassSchedules.GetByIdAsync(classScheduleId);
            if (schedule == null) return;

            int currentBookedCount = await _unitOfWork.Bookings.GetConfirmedBookingsCountAsync(classScheduleId);

            if (currentBookedCount < schedule.MaxCapacity)
            {
                var nextInLine = await _unitOfWork.Waitlists.GetFirstWaitlistEntryForClassAsync(classScheduleId);

                if (nextInLine != null)
                {
                    var newBooking = new Booking
                    {
                        UserId = nextInLine.UserId,
                        ClassScheduleId = schedule.Id,
                        CreditsDeducted = nextInLine.CreditsDeducted,
                        BookingDate = nextInLine.WaitlistDate,
                        Status = BookingStatus.Confirmed.ToString(),
                        CheckedIn = false,
                    };
                    await _unitOfWork.Bookings.AddAsync(newBooking);

                    _unitOfWork.Waitlists.Delete(nextInLine);
                    await _unitOfWork.CompleteAsync();
                    await ProcessWaitlistAfterCancellation(classScheduleId);
                }
            }
        }

        public async Task FinalizeClassAndRefundWaitlist(int classScheduleId)
        {
            var confirmedBookings = await _unitOfWork.Bookings.GetAllAsync();
            foreach (var booking in confirmedBookings
                .Where(b => b.ClassScheduleId == classScheduleId && b.Status == BookingStatus.Confirmed.ToString()))
            {
                booking.Status = booking.CheckedIn ? BookingStatus.CheckedIn.ToString() : BookingStatus.Completed.ToString();
                _unitOfWork.Bookings.Update(booking);
            }

            var waitlistEntries = await _unitOfWork.Waitlists.GetAllAsync();
            foreach (var entry in waitlistEntries
                .Where(e => e.ClassScheduleId == classScheduleId && !e.IsCreditRefunded))
            {
                var userPackage = await _unitOfWork.UserPackages.GetByIdAsync(entry.UserId);
                if (userPackage != null)
                {
                    userPackage.RemainingCredits += entry.CreditsDeducted;
                    _unitOfWork.UserPackages.Update(userPackage);
                }

                entry.IsCreditRefunded = true;
                _unitOfWork.Waitlists.Delete(entry);
            }

            await _unitOfWork.CompleteAsync();
        }
    }
}
