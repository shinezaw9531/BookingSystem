using Application.Features.Auth.Profile.Models;
using MediatR;

namespace Application.Features.Auth.Profile.Queries
{
    public class GetProfileQuery : IRequest<GetProfileResponse>
    {
        public int UserId { get; set; }

        public GetProfileQuery(int userId)
        {
            UserId = userId;
        }
    }
}
