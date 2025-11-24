using Application.Interfaces.Repositories;
using Domain.Entity;
using Infrastructure.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class ClassScheduleRepository : IClassScheduleRepository
    {
        private readonly BookingSystemContext _dbContext;

        public ClassScheduleRepository(BookingSystemContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ClassSchedule> GetByIdAsync(int id)
        {
            return await _dbContext.ClassSchedules.FindAsync(id);
        }

        public async Task<IReadOnlyList<ClassSchedule>> GetAllAsync()
        {
            return await _dbContext.ClassSchedules.ToListAsync();
        }

        public async Task AddAsync(ClassSchedule entity)
        {
            await _dbContext.ClassSchedules.AddAsync(entity);
        }

        public void Update(ClassSchedule entity)
        {
            _dbContext.ClassSchedules.Update(entity);
        }

        public void Delete(ClassSchedule entity)
        {
            _dbContext.ClassSchedules.Remove(entity);
        }

        public async Task<IReadOnlyList<ClassSchedule>> GetSchedulesByCountryAsync(int countryId)
        {
            return await _dbContext.ClassSchedules
                .Where(cs => cs.CountryId == countryId)
                .OrderBy(cs => cs.StartTime)
                .ToListAsync();
        }
    }
}
