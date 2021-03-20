using System.Linq;
using GreenFlux.Application.Constants;
using GreenFlux.Application.Models;
using GreenFlux.Application.Services;

namespace GreenFlux.Application.Mappers
{
    public interface IChargeStationModelMapper
    {
        ChargeStation Map(Domain.Models.ChargeStation chargeStationDomainModel);
    }

    public class ChargeStationModelMapper : IChargeStationModelMapper
    {
        private readonly ILinksService _linksService;
        private readonly IConnectorModelMapper _connectorModelMapper;

        public ChargeStationModelMapper(ILinksService linksService, IConnectorModelMapper connectorModelMapper)
        {
            _linksService = linksService;
            _connectorModelMapper = connectorModelMapper;
        }

        public ChargeStation Map(Domain.Models.ChargeStation chargeStationDomainModel)
        {
            var chargeStation = new ChargeStation
            {
                Identifier = chargeStationDomainModel.Identifier,
                Name = chargeStationDomainModel.Name,
                Connectors = chargeStationDomainModel.Connectors?.Select(_connectorModelMapper.Map) ?? Enumerable.Empty<Connector>()
            };

            chargeStation.Links.Add(new Link(RelationShips.All, _linksService.LinkToChargeStations(chargeStationDomainModel.Group.Identifier), Method.Get));
            chargeStation.Links.Add(new Link(RelationShips.Self, _linksService.LinkToChargeStation(chargeStationDomainModel), Method.Get));
            chargeStation.Links.Add(new Link(RelationShips.Self, _linksService.LinkToChargeStation(chargeStationDomainModel), Method.Put));
            chargeStation.Links.Add(new Link(RelationShips.Self, _linksService.LinkToChargeStation(chargeStationDomainModel), Method.Delete));

            return chargeStation;
        }
    }
}
