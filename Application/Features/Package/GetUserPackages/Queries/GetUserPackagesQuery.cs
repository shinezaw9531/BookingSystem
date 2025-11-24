using Application.Features.Package.GetUserPackages.Models;
using MediatR;

namespace Application.Features.Package.GetUserPackages.Queries
{
    public record GetUserPackagesQuery(int UserId) : IRequest<IReadOnlyList<UserPackageDto>>;
}
