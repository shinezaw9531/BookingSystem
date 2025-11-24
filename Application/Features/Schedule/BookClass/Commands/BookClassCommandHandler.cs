using Application.Interfaces.Repositories;
using Domain;
using Domain.Entity;
using Domain.Enums;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Application.Features.Schedule.BookClass.Commands
{
    public class BookClassCommandHandler : IRequestHandler<BookClassCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDistributedCache _cache;

        public BookClassCommandHandler(IUnitOfWork unitOfWork, IDistributedCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<string> Handle(BookClassCommand request, CancellationToken cancellationToken)
        {
            var schedule = await _unitOfWork.ClassSchedules.GetByIdAsync(request.ClassScheduleId)
                ?? throw new NotFoundException(nameof(ClassSchedule), request.ClassScheduleId);

            if (schedule.StartTime < DateTime.UtcNow)
                throw new BusinessRuleException("Class already started or passed.");

            if (await _unitOfWork.Bookings.HasOverlapBookingAsync(request.UserId, schedule.StartTime, schedule.EndTime))
                throw new BusinessRuleException("Cannot book: overlaps with an existing confirmed class.");

            if (await _unitOfWork.Bookings.GetUserConfirmedBookingForScheduleAsync(request.UserId, request.ClassScheduleId) != null)
                return "You are already confirmed for this class.";
            if (await _unitOfWork.Waitlists.GetWaitlistEntryByUserAndClassAsync(request.UserId, request.ClassScheduleId) != null)
                return "You are already on the waitlist for this class.";

            var activePackages = await _unitOfWork.UserPackages.GetUserActivePackagesAsync(request.UserId, schedule.CountryId);
            var targetPackage = activePackages
                .OrderBy(p => p.ExpiryDate)
                .FirstOrDefault(p => p.RemainingCredits >= schedule.RequiredCredits);

            if (targetPackage == null)
                throw new BusinessRuleException("Insufficient or incompatible credits available for this class/country.");

            string lockKey = $"booking_lock:{request.ClassScheduleId}";
            string lockValue = Guid.NewGuid().ToString();

            bool lockAcquired = await _cache.GetStringAsync(lockKey) == null;
            if (lockAcquired)
            {
                await _cache.SetStringAsync(lockKey, lockValue, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5) });
            }
            else
            {
                return "System busy. Please try booking again.";
            }

            try
            {
                int currentBookedCount = await _unitOfWork.Bookings.GetConfirmedBookingsCountAsync(request.ClassScheduleId);
                bool isFull = currentBookedCount >= schedule.MaxCapacity;

                if (isFull)
                {
                    await AddToWaitlist(request.UserId, schedule, targetPackage);
                    return "Class is full. Credits reserved. Added to waitlist.";
                }
                else
                {
                    await CreateConfirmedBooking(request.UserId, schedule, targetPackage);
                    return "Booking successful!";
                }
            }
            finally
            {
                if (await _cache.GetStringAsync(lockKey) == lockValue)
                {
                    await _cache.RemoveAsync(lockKey);
                }
            }
        }

        private async Task CreateConfirmedBooking(int userId, ClassSchedule schedule, UserPackage targetPackage)
        {
            targetPackage.RemainingCredits -= schedule.RequiredCredits;
            _unitOfWork.UserPackages.Update(targetPackage);

            var newBooking = new Booking
            {
                UserId = userId,
                ClassScheduleId = schedule.Id,
                CreditsDeducted = schedule.RequiredCredits,
                BookingDate = DateTime.UtcNow,
                Status = BookingStatus.Confirmed.ToString(),
                CheckedIn = false,
                UserPackageId = targetPackage.Id
            };

            await _unitOfWork.Bookings.AddAsync(newBooking);
            await _unitOfWork.CompleteAsync();

            BackgroundJob.Schedule(
                () => _unitOfWork.Bookings.Update(newBooking),
                schedule.EndTime.AddMinutes(1) - DateTime.UtcNow
            );
        }

        private async Task AddToWaitlist(int userId, ClassSchedule schedule, UserPackage targetPackage)
        {
            targetPackage.RemainingCredits -= schedule.RequiredCredits;
            _unitOfWork.UserPackages.Update(targetPackage);

            int nextOrder = await _unitOfWork.Waitlists.GetNextWaitlistOrderAsync(schedule.Id);

            var waitlistEntry = new Waitlist
            {
                UserId = userId,
                ClassScheduleId = schedule.Id,
                WaitlistDate = DateTime.UtcNow,
                Order = nextOrder,
                CreditsDeducted = schedule.RequiredCredits,
                IsCreditRefunded = false
            };
            await _unitOfWork.Waitlists.AddAsync(waitlistEntry);
            await _unitOfWork.CompleteAsync();
        }
    }
}
