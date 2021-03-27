using GreenFlux.Application.Constants;
using GreenFlux.Application.DtoModels;
using GreenFlux.Application.Services;
using GreenFlux.Domain.Models;

namespace GreenFlux.Application.Mappers
{
    public interface IConnectorDtoMapper
    {
        ConnectorDto Map(ChargeStation chargeStation, short connectorId);
    }

    public class ConnectorDtoMapper : IConnectorDtoMapper
    {
        private readonly ILinksService _linksService;

        public ConnectorDtoMapper(ILinksService linksService)
        {
            _linksService = linksService;
        }

        public ConnectorDto Map(ChargeStation chargeStation, short connectorId)
        {
            var maxCapacityInAmps = chargeStation.GetCapacity(connectorId);

            if (maxCapacityInAmps == null)
            {
                return null;
            }

            var connectorDto = new ConnectorDto
            {
                Id = connectorId,
                MaxCurrentInAmps = maxCapacityInAmps.Value
            };

            connectorDto.Links.Add(new LinkDto(RelationShips.All, _linksService.LinkToConnectors(chargeStation), Method.Get));
            connectorDto.Links.Add(new LinkDto(RelationShips.Self, _linksService.LinkToConnector(chargeStation, connectorId), Method.Get));
            connectorDto.Links.Add(new LinkDto(RelationShips.Self, _linksService.LinkToConnector(chargeStation, connectorId), Method.Put));
            connectorDto.Links.Add(new LinkDto(RelationShips.Self, _linksService.LinkToConnector(chargeStation, connectorId), Method.Delete));

            return connectorDto;
        }
    }
}
