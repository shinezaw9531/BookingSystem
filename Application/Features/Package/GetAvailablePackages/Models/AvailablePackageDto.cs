namespace Application.Features.Package.GetAvailablePackages.Models
{
    public class AvailablePackageDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Credits { get; set; }
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
    }
}
