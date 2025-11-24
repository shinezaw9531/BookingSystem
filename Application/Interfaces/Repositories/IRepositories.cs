using Domain.Entity;

namespace Application.Interfaces.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }

    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
    }

    public interface ICountryRepository : IBaseRepository<Country>
    {
        Task<Country> GetByCodeAsync(string countryCode);
    }

    public interface IPackageRepository : IBaseRepository<Package>
    {
        Task<IReadOnlyList<Package>> GetAvailablePackagesByCountryAsync(int countryId);
    }

    public interface IUserPackageRepository : IBaseRepository<UserPackage>
    {
        Task<IReadOnlyList<UserPackage>> GetUserActivePackagesAsync(int userId, int countryId);
        Task<IReadOnlyList<UserPackage>> GetUserAllPackagesAsync(int userId);
    }

    public interface IClassScheduleRepository : IBaseRepository<ClassSchedule>
    {
        Task<IReadOnlyList<ClassSchedule>> GetSchedulesByCountryAsync(int countryId);
    }

    public interface IBookingRepository : IBaseRepository<Booking>
    {
        Task<bool> HasOverlapBookingAsync(int userId, DateTime startTime, DateTime endTime);
        Task<IReadOnlyList<Booking>> GetUserConfirmedBookingsAsync(int userId);
        Task<int> GetConfirmedBookingsCountAsync(int classScheduleId);
        Task<Booking> GetBookingDetailsAsync(int bookingId);
        Task<Booking> GetUserConfirmedBookingForScheduleAsync(int userId, int classScheduleId);
        Task<List<Booking>> GetUserBookingsForSchedulesAsync(IEnumerable<int> classScheduleIds);
    }

    public interface IWaitlistRepository : IBaseRepository<Waitlist>
    {
        Task<Waitlist> GetFirstWaitlistEntryForClassAsync(int classScheduleId); // FIFO (lowest Order)
        Task<int> GetNextWaitlistOrderAsync(int classScheduleId);
        Task<Waitlist> GetWaitlistEntryByUserAndClassAsync(int userId, int classScheduleId);
    }

    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        ICountryRepository Countries { get; }
        IPackageRepository Packages { get; }
        IUserPackageRepository UserPackages { get; }
        IClassScheduleRepository ClassSchedules { get; }
        IBookingRepository Bookings { get; }
        IWaitlistRepository Waitlists { get; }
        Task<int> CompleteAsync(CancellationToken cancellationToken = default);
    }

}
