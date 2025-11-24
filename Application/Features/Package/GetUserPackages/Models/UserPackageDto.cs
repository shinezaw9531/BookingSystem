namespace Application.Features.Package.GetUserPackages.Models
{
    public class UserPackageDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public int InitialCredits { get; set; }
        public int RemainingCredits { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive => RemainingCredits > 0 && ExpiryDate > DateTime.UtcNow;
    }
}
