using Application.Features.Schedule.GetSchedules.Models;
using MediatR;

namespace Application.Features.Schedule.GetSchedules.Queries
{
    public record GetSchedulesQuery(string CountryCode) : IRequest<IReadOnlyList<ScheduleDto>>;
}
