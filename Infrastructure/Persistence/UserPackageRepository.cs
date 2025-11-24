using Application.Interfaces.Repositories;
using Domain.Entity;
using Infrastructure.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class UserPackageRepository : IUserPackageRepository
    {
        private readonly BookingSystemContext _dbContext;

        public UserPackageRepository(BookingSystemContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserPackage> GetByIdAsync(int id)
        {
            return await _dbContext.UserPackages.FindAsync(id);
        }

        public async Task<IReadOnlyList<UserPackage>> GetAllAsync()
        {
            return await _dbContext.UserPackages.ToListAsync();
        }

        public async Task AddAsync(UserPackage entity)
        {
            await _dbContext.UserPackages.AddAsync(entity);
        }

        public void Update(UserPackage entity)
        {
            _dbContext.UserPackages.Update(entity);
        }

        public void Delete(UserPackage entity)
        {
            _dbContext.UserPackages.Remove(entity);
        }


        public async Task<IReadOnlyList<UserPackage>> GetUserActivePackagesAsync(int userId, int countryId)
        {
            return await _dbContext.UserPackages
                .Include(up => up.Package)
                .Where(up => up.UserId == userId &&
                             up.ExpiryDate > DateTime.UtcNow &&
                             up.RemainingCredits > 0 &&
                             up.Package.CountryId == countryId)
                .OrderBy(up => up.ExpiryDate)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<UserPackage>> GetUserAllPackagesAsync(int userId)
        {
            return await _dbContext.UserPackages
                .Where(up => up.UserId == userId)
                .Include(up => up.Package)
                .ToListAsync();
        }
    }
}
