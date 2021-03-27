using System.Linq;
using GreenFlux.Application.Constants;
using GreenFlux.Application.DtoModels;
using GreenFlux.Application.Services;
using GreenFlux.Domain.Models;

namespace GreenFlux.Application.Mappers
{
    public interface IConnectorsDtoMapper
    {
        ConnectorsDto Map(ChargeStation chargeStation);
    }

    public class ConnectorsDtoMapper : IConnectorsDtoMapper
    {
        private readonly IConnectorDtoMapper _connectorDtoMapper;
        private readonly ILinksService _linksService;

        public ConnectorsDtoMapper(IConnectorDtoMapper connectorDtoMapper, ILinksService linksService)
        {
            _connectorDtoMapper = connectorDtoMapper;
            _linksService = linksService;
        }
        
        public ConnectorsDto Map(ChargeStation chargeStation)
        {
            var connectorsDto = new ConnectorsDto
            {

                Values = chargeStation.ConnectorCapacities.Select(c => _connectorDtoMapper.Map(chargeStation, c.id))
            };

            connectorsDto.Links.Add(new LinkDto(RelationShips.Self, _linksService.LinkToConnectors(chargeStation), Method.Post));
            connectorsDto.Links.Add(new LinkDto(RelationShips.ChargeStation, _linksService.LinkToChargeStation(chargeStation), Method.Get));

            return connectorsDto;
        }
    }
}
