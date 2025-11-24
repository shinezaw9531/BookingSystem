using MediatR;

namespace Application.Features.Schedule.BookClass.Commands
{
    public record BookClassCommand(int UserId, int ClassScheduleId) : IRequest<string>;
}
