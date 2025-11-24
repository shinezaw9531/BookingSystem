using MediatR;

namespace Application.Features.Schedule.CancelBooking.Commands
{
    public record CancelBookingCommand(int UserId, int BookingId) : IRequest<string>;
}
