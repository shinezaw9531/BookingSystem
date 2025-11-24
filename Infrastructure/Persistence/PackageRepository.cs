using Application.Interfaces.Repositories;
using Domain.Entity;
using Infrastructure.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class PackageRepository : IPackageRepository
    {
        private readonly BookingSystemContext _dbContext;

        public PackageRepository(BookingSystemContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Package> GetByIdAsync(int id)
        {
            return await _dbContext.Packages.FindAsync(id);
        }

        public async Task<IReadOnlyList<Package>> GetAllAsync()
        {
            return await _dbContext.Packages.ToListAsync();
        }

        public async Task AddAsync(Package entity)
        {
            await _dbContext.Packages.AddAsync(entity);
        }

        public void Update(Package entity)
        {
            _dbContext.Packages.Update(entity);
        }

        public void Delete(Package entity)
        {
            _dbContext.Packages.Remove(entity);
        }

        public async Task<IReadOnlyList<Package>> GetAvailablePackagesByCountryAsync(int countryId)
        {
            return await _dbContext.Packages
                .Where(p => p.CountryId == countryId)
                .ToListAsync();
        }
    }
}
