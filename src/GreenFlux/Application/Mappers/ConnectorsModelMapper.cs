using System.Linq;
using GreenFlux.Application.Constants;
using GreenFlux.Application.Models;
using GreenFlux.Application.Services;

namespace GreenFlux.Application.Mappers
{
    public interface IConnectorsModelMapper
    {
        Connectors Map(Domain.Models.ChargeStation chargeStationDomainModel);
    }

    public class ConnectorsModelMapper : IConnectorsModelMapper
    {
        private readonly IConnectorModelMapper _connectorModelMapper;
        private readonly ILinksService _linksService;

        public ConnectorsModelMapper(IConnectorModelMapper connectorModelMapper, ILinksService linksService)
        {
            _connectorModelMapper = connectorModelMapper;
            _linksService = linksService;
        }
        
        public Connectors Map(Domain.Models.ChargeStation chargeStationDomainModel)
        {
            var connectors = new Connectors
            {
                Values = chargeStationDomainModel.Connectors?.Select(_connectorModelMapper.Map) ?? Enumerable.Empty<Connector>()
            };

            connectors.Links.Add(new Link(RelationShips.Self, _linksService.LinkToConnectors(chargeStationDomainModel), Method.Post));
            connectors.Links.Add(new Link(RelationShips.ChargeStation, _linksService.LinkToChargeStation(chargeStationDomainModel), Method.Get));

            return connectors;
        }
    }
}
