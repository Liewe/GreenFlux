using GreenFlux.Application.Constants;
using GreenFlux.Application.Models;
using GreenFlux.Application.Services;

namespace GreenFlux.Application.Mappers
{
    public interface IGroupModelMapper
    {
        Group Map(Domain.Models.Group from);
    }

    public class GroupModelMapper : IGroupModelMapper
    {
        private readonly ILinksService _linksService;

        public GroupModelMapper(ILinksService linksService)
        {
            _linksService = linksService;
        }

        public Group Map(Domain.Models.Group groupDomainModel)
        {
            var group = new Group
            {
                Identifier = groupDomainModel.Identifier,
                Name = groupDomainModel.Name,
                CapacityInAmps = groupDomainModel.CapacityInAmps
            };

            group.Links.Add(new Link(RelationShips.All, _linksService.LinkToGroups(), Method.Get));
            group.Links.Add(new Link(RelationShips.Self, _linksService.LinkToGroup(group.Identifier), Method.Get));
            group.Links.Add(new Link(RelationShips.Self, _linksService.LinkToGroup(group.Identifier), Method.Put));
            group.Links.Add(new Link(RelationShips.Self, _linksService.LinkToGroup(group.Identifier), Method.Delete));
            group.Links.Add(new Link(RelationShips.ChargeStation, _linksService.LinkToChargeStations(group.Identifier), Method.Get));

            return group;
        }
    }
}
