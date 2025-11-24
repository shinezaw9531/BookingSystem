using Application.Interfaces.Repositories;
using Domain.Entity;
using Infrastructure.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class CountryRepository : ICountryRepository
    {
        private readonly BookingSystemContext _dbContext;

        public CountryRepository(BookingSystemContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Country> GetByIdAsync(int id)
        {
            return await _dbContext.Countries.FindAsync(id);
        }

        public async Task<IReadOnlyList<Country>> GetAllAsync()
        {
            return await _dbContext.Countries.ToListAsync();
        }

        public async Task AddAsync(Country entity)
        {
            await _dbContext.Countries.AddAsync(entity);
        }

        public void Update(Country entity)
        {
            _dbContext.Countries.Update(entity);
        }

        public void Delete(Country entity)
        {
            _dbContext.Countries.Remove(entity);
        }

        // Specific method for ICountryRepository
        public async Task<Country> GetByCodeAsync(string countryCode)
        {
            return await _dbContext.Countries.FirstOrDefaultAsync(c => c.Code == countryCode);
        }
    }
}
