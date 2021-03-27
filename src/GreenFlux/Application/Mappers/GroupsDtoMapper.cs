using System.Collections.Generic;
using System.Linq;
using GreenFlux.Application.Constants;
using GreenFlux.Application.DtoModels;
using GreenFlux.Application.Services;
using GreenFlux.Domain.Models;

namespace GreenFlux.Application.Mappers
{
    public interface IGroupsDtoMapper
    {
        GroupsDto Map(IEnumerable<Group> from);
    }

    public class GroupsDtoMapper : IGroupsDtoMapper
    {
        private readonly IGroupDtoMapper _groupDtoMapper;
        private readonly ILinksService _linksService;

        public GroupsDtoMapper(IGroupDtoMapper groupDtoMapper, ILinksService linksService)
        {
            _groupDtoMapper = groupDtoMapper;
            _linksService = linksService;
        }

        public GroupsDto Map(IEnumerable<Group> @group)
        {
            var groupsDto = new GroupsDto
            {
                Values = group?.Select(_groupDtoMapper.Map) ?? Enumerable.Empty<GroupDto>()
            };
            
            groupsDto.Links.Add(new LinkDto(RelationShips.Self, _linksService.LinkToGroups(), Method.Get));
            groupsDto.Links.Add(new LinkDto(RelationShips.Group, _linksService.LinkToGroups(), Method.Post));

            return groupsDto;
        }
    }
}
