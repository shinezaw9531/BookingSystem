using MediatR;

namespace Application.Features.Schedule.CheckIn.Commands
{
    public record CheckInCommand(int UserId, int BookingId) : IRequest<string>;
}
