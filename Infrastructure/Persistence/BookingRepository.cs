using Application.Interfaces.Repositories;
using Domain.Entity;
using Domain.Enums;
using Infrastructure.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingSystemContext _dbContext;

        public BookingRepository(BookingSystemContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Booking> GetByIdAsync(int id)
        {
            return await _dbContext.Bookings.FindAsync(id);
        }

        public async Task<IReadOnlyList<Booking>> GetAllAsync()
        {
            return await _dbContext.Bookings.ToListAsync();
        }

        public async Task AddAsync(Booking entity)
        {
            await _dbContext.Bookings.AddAsync(entity);
        }

        public void Update(Booking entity)
        {
            _dbContext.Bookings.Update(entity);
        }

        public void Delete(Booking entity)
        {
            _dbContext.Bookings.Remove(entity);
        }

        public async Task<bool> HasOverlapBookingAsync(int userId, DateTime startTime, DateTime endTime)
        {
            return await _dbContext.Bookings
                .Include(b => b.ClassSchedule)
                .AnyAsync(b => b.UserId == userId &&
                               b.Status == BookingStatus.Confirmed.ToString() &&
                               b.ClassSchedule.StartTime < endTime &&
                               b.ClassSchedule.EndTime > startTime);
        }

        public async Task<int> GetConfirmedBookingsCountAsync(int classScheduleId)
        {
            return await _dbContext.Bookings
                .CountAsync(b => b.ClassScheduleId == classScheduleId &&
                                 b.Status == BookingStatus.Confirmed.ToString());
        }

        public async Task<Booking> GetBookingDetailsAsync(int bookingId)
        {
            return await _dbContext.Bookings
                .Include(b => b.ClassSchedule)
                .Include(b => b.UserPackage)
                .FirstOrDefaultAsync(b => b.Id == bookingId);
        }

        public async Task<Booking> GetUserConfirmedBookingForScheduleAsync(int userId, int classScheduleId)
        {
            return await _dbContext.Bookings
                .FirstOrDefaultAsync(b => b.UserId == userId &&
                                          b.ClassScheduleId == classScheduleId &&
                                          b.Status == BookingStatus.Confirmed.ToString());
        }
        public async Task<List<Booking>> GetUserBookingsForSchedulesAsync(IEnumerable<int> classScheduleIds)
        {
            return await _dbContext.Bookings
                .Where(b => classScheduleIds.Contains(b.ClassScheduleId) &&
                            b.Status == BookingStatus.Confirmed.ToString())
                .ToListAsync();
        }

        public Task<IReadOnlyList<Booking>> GetUserConfirmedBookingsAsync(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
