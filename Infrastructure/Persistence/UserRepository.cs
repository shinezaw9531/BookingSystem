using Application.Interfaces.Repositories;
using Domain.Entity;
using Infrastructure.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Persistence
{
    public class UserRepository : IUserRepository
    {
        private readonly BookingSystemContext _dbContext;

        public UserRepository(BookingSystemContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _dbContext.Users.FindAsync(id);
        }

        public async Task<IReadOnlyList<User>> GetAllAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task AddAsync(User entity)
        {
            await _dbContext.Users.AddAsync(entity);
        }

        public void Update(User entity)
        {
            _dbContext.Users.Update(entity);
        }

        public void Delete(User entity)
        {
            _dbContext.Users.Remove(entity);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

    }
}
