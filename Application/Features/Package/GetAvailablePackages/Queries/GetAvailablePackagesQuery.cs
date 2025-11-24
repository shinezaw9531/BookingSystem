using Application.Features.Package.GetAvailablePackages.Models;
using MediatR;

namespace Application.Features.Package.GetAvailablePackages.Queries
{
    public record GetAvailablePackagesQuery(string CountryCode) : IRequest<IReadOnlyList<AvailablePackageDto>>;
}
