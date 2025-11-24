using Application.Interfaces.Repositories;
using Domain.Entity;

namespace Infrastructure.Persistence
{
    public class MockUserRepository : IUserRepository
    {
        private static readonly List<User> _users = new List<User>();
        private static int _nextId = 1;

        public Task<User> GetByIdAsync(int id) => Task.FromResult(_users.FirstOrDefault(u => u.Id == id));
        public Task<IReadOnlyList<User>> GetAllAsync() => Task.FromResult((IReadOnlyList<User>)_users);

        public Task AddAsync(User entity)
        {
            entity.Id = _nextId++;
            _users.Add(entity);
            return Task.CompletedTask;
        }

        public void Update(User entity)
        {
            var existing = _users.FirstOrDefault(u => u.Id == entity.Id);
            if (existing != null)
            {
                existing.IsEmailVerified = entity.IsEmailVerified;
            }
        }

        public void Delete(User entity) => _users.RemoveAll(u => u.Id == entity.Id);

        public Task<User> GetByEmailAsync(string email)
            => Task.FromResult(_users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)));
    }
}
