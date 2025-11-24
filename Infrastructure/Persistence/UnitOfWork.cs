using Application.Interfaces.Repositories;
using Infrastructure.DatabaseContexts;

namespace Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BookingSystemContext _dbContext;
        public IUserRepository Users { get; }

        public ICountryRepository Countries { get; }

        public IPackageRepository Packages { get; }

        public IUserPackageRepository UserPackages { get; }

        public IClassScheduleRepository ClassSchedules { get; }

        public IBookingRepository Bookings { get; }

        public IWaitlistRepository Waitlists { get; }

        public UnitOfWork(BookingSystemContext dbContext)
        {
            _dbContext = dbContext;
            Users = new UserRepository(_dbContext);
            Countries = new CountryRepository(_dbContext);
            Packages = new PackageRepository(_dbContext);
            UserPackages = new UserPackageRepository(_dbContext);
            ClassSchedules = new ClassScheduleRepository(_dbContext);
            Bookings = new BookingRepository(_dbContext);
            Waitlists = new WaitlistRepository(_dbContext);
        }

        public Task<int> CompleteAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
