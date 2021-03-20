using System.Collections.Generic;
using System.Linq;
using GreenFlux.Application.Constants;
using GreenFlux.Application.Models;
using GreenFlux.Application.Services;

namespace GreenFlux.Application.Mappers
{
    public interface IGroupsModelMapper
    {
        Groups Map(IEnumerable<Domain.Models.Group> from);
    }

    public class GroupsModelMapper : IGroupsModelMapper
    {
        private readonly IGroupModelMapper _groupModelMapper;
        private readonly ILinksService _linksService;

        public GroupsModelMapper(IGroupModelMapper groupModelMapper, ILinksService linksService)
        {
            _groupModelMapper = groupModelMapper;
            _linksService = linksService;
        }

        public Groups Map(IEnumerable<Domain.Models.Group> groupDomainModel)
        {
            var groups = new Groups
            {
                Values = groupDomainModel?.Select(_groupModelMapper.Map) ?? Enumerable.Empty<Group>()
            };
            
            groups.Links.Add(new Link(RelationShips.Self, _linksService.LinkToGroups(), Method.Get));
            groups.Links.Add(new Link(RelationShips.Group, _linksService.LinkToGroups(), Method.Post));

            return groups;
        }
    }
}
