using System.Linq;
using GreenFlux.Application.Constants;
using GreenFlux.Application.Models;
using GreenFlux.Application.Services;

namespace GreenFlux.Application.Mappers
{
    public interface IChargeStationsModelMapper
    {
        ChargeStations Map(Domain.Models.Group groupDomainModel);
    }

    public class ChargeStationsModelMapper : IChargeStationsModelMapper
    {
        private readonly IChargeStationModelMapper _chargeStationModelMapper;
        private readonly ILinksService _linksService;

        public ChargeStationsModelMapper(IChargeStationModelMapper chargeStationModelMapper, ILinksService linksService)
        {
            _chargeStationModelMapper = chargeStationModelMapper;
            _linksService = linksService;
        }
        
        public ChargeStations Map(Domain.Models.Group groupDomainModel)
        {
            var chargeStations = new ChargeStations
            {
                Values = groupDomainModel.ChargeStations?.Select(_chargeStationModelMapper.Map) ?? Enumerable.Empty<ChargeStation>()
            };

            chargeStations.Links.Add(new Link(RelationShips.Self, _linksService.LinkToChargeStations(groupDomainModel.Id), Method.Post));
            chargeStations.Links.Add(new Link(RelationShips.Group, _linksService.LinkToGroup(groupDomainModel.Id), Method.Get));

            return chargeStations;
        }
    }
}
