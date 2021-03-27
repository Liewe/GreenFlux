using GreenFlux.Application.Constants;
using GreenFlux.Application.DtoModels;
using GreenFlux.Application.Services;
using GreenFlux.Domain.Models;

namespace GreenFlux.Application.Mappers
{
    public interface IGroupDtoMapper
    {
        GroupDto Map(Group group);
    }

    public class GroupDtoMapper : IGroupDtoMapper
    {
        private readonly ILinksService _linksService;

        public GroupDtoMapper(ILinksService linksService)
        {
            _linksService = linksService;
        }

        public GroupDto Map(Group group)
        {
            var groupDto = new GroupDto
            {
                Id = group.Id,
                Name = group.Name,
                CapacityInAmps = group.CapacityInAmps
            };

            groupDto.Links.Add(new LinkDto(RelationShips.All, _linksService.LinkToGroups(), Method.Get));
            groupDto.Links.Add(new LinkDto(RelationShips.Self, _linksService.LinkToGroup(groupDto.Id), Method.Get));
            groupDto.Links.Add(new LinkDto(RelationShips.Self, _linksService.LinkToGroup(groupDto.Id), Method.Put));
            groupDto.Links.Add(new LinkDto(RelationShips.Self, _linksService.LinkToGroup(groupDto.Id), Method.Delete));
            groupDto.Links.Add(new LinkDto(RelationShips.ChargeStation, _linksService.LinkToChargeStations(groupDto.Id), Method.Get));

            return groupDto;
        }
    }
}
