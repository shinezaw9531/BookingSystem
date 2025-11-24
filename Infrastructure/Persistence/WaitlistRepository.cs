using Application.Interfaces.Repositories;
using Domain.Entity;
using Infrastructure.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class WaitlistRepository : IWaitlistRepository
    {
        private readonly BookingSystemContext _dbContext;

        public WaitlistRepository(BookingSystemContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Waitlist> GetByIdAsync(int id)
        {
            return await _dbContext.Waitlists.FindAsync(id);
        }

        public async Task<IReadOnlyList<Waitlist>> GetAllAsync()
        {
            return await _dbContext.Waitlists.ToListAsync();
        }

        public async Task AddAsync(Waitlist entity)
        {
            await _dbContext.Waitlists.AddAsync(entity);
        }

        public void Update(Waitlist entity)
        {
            _dbContext.Waitlists.Update(entity);
        }

        public void Delete(Waitlist entity)
        {
            _dbContext.Waitlists.Remove(entity);
        }

        public async Task<Waitlist> GetFirstWaitlistEntryForClassAsync(int classScheduleId)
        {
            return await _dbContext.Waitlists
                .Where(w => w.ClassScheduleId == classScheduleId)
                .OrderBy(w => w.Order)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetNextWaitlistOrderAsync(int classScheduleId)
        {
            int? maxOrder = await _dbContext.Waitlists
                       .Where(w => w.ClassScheduleId == classScheduleId)
                       .MaxAsync(w => (int?)w.Order);

            return (maxOrder ?? 0) + 1;
        }

        public async Task<Waitlist> GetWaitlistEntryByUserAndClassAsync(int userId, int classScheduleId)
        {
            return await _dbContext.Waitlists
               .FirstOrDefaultAsync(w => w.UserId == userId &&
                                         w.ClassScheduleId == classScheduleId);
        }
    }
}
