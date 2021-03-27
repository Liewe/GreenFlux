using System.Linq;
using GreenFlux.Application.Constants;
using GreenFlux.Application.DtoModels;
using GreenFlux.Application.Services;
using GreenFlux.Domain.Models;

namespace GreenFlux.Application.Mappers
{
    public interface IChargeStationDtoMapper
    {
        ChargeStationDto Map(ChargeStation chargeStation);
    }

    public class ChargeStationDtoMapper : IChargeStationDtoMapper
    {
        private readonly ILinksService _linksService;
        private readonly IConnectorDtoMapper _connectorDtoMapper;

        public ChargeStationDtoMapper(ILinksService linksService, IConnectorDtoMapper connectorDtoMapper)
        {
            _linksService = linksService;
            _connectorDtoMapper = connectorDtoMapper;
        }

        public ChargeStationDto Map(ChargeStation chargeStation)
        {
            var chargeStationDto = new ChargeStationDto
            {
                Id = chargeStation.Id,
                Name = chargeStation.Name,
                Connectors = chargeStation.ConnectorCapacities.Select(c => _connectorDtoMapper.Map(chargeStation, c.id)) 
            };

            chargeStationDto.Links.Add(new LinkDto(RelationShips.All, _linksService.LinkToChargeStations(chargeStation.Group.Id), Method.Get));
            chargeStationDto.Links.Add(new LinkDto(RelationShips.Self, _linksService.LinkToChargeStation(chargeStation), Method.Get));
            chargeStationDto.Links.Add(new LinkDto(RelationShips.Self, _linksService.LinkToChargeStation(chargeStation), Method.Put));
            chargeStationDto.Links.Add(new LinkDto(RelationShips.Self, _linksService.LinkToChargeStation(chargeStation), Method.Delete));
                         
            return chargeStationDto;
        }
    }
}
