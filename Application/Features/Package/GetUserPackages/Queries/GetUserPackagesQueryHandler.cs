using Application.Features.Package.GetUserPackages.Models;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.Features.Package.GetUserPackages.Queries
{
    public class GetUserPackagesQueryHandler : IRequestHandler<GetUserPackagesQuery, IReadOnlyList<UserPackageDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserPackagesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<UserPackageDto>> Handle(GetUserPackagesQuery request, CancellationToken cancellationToken)
        {
            var userPackages = await _unitOfWork.UserPackages.GetUserAllPackagesAsync(request.UserId);

            if (userPackages == null || !userPackages.Any())
            {
                return new List<UserPackageDto>();
            }

            var packageDtos = userPackages.Select(p => new UserPackageDto
            {
                Id = p.Id,
                UserId = p.UserId,
                PackageName = p.Package?.Name ?? "Unknown Package",
                InitialCredits = p.Package?.InitialCredits ?? 0,
                RemainingCredits = p.RemainingCredits,
                PurchaseDate = p.PurchaseDate,
                ExpiryDate = p.ExpiryDate
            }).ToList();

            return packageDtos;
        }
    }
}
