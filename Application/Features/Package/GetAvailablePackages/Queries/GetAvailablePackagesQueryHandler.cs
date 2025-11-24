using Application.Features.Package.GetAvailablePackages.Models;
using Application.Interfaces.Repositories;
using Domain;
using MediatR;

namespace Application.Features.Package.GetAvailablePackages.Queries
{
    public class GetAvailablePackagesQueryHandler : IRequestHandler<GetAvailablePackagesQuery, IReadOnlyList<AvailablePackageDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAvailablePackagesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<AvailablePackageDto>> Handle(GetAvailablePackagesQuery request, CancellationToken cancellationToken)
        {
            var country = await _unitOfWork.Countries.GetByCodeAsync(request.CountryCode);

            if (country == null)
            {
                throw new NotFoundException("Country", request.CountryCode);
            }

            var packages = await _unitOfWork.Packages.GetAvailablePackagesByCountryAsync(country.Id);

            if (packages == null || !packages.Any())
            {
                return new List<AvailablePackageDto>();
            }

            var packageDtos = packages.Select(p => new AvailablePackageDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = "Digital Credit Package",
                Credits = p.InitialCredits,
                Price = p.Price,
                DurationDays = 0
            }).ToList();

            return packageDtos;
        }
    }
}
