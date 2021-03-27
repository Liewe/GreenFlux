using System.Linq;
using GreenFlux.Application.Constants;
using GreenFlux.Application.DtoModels;
using GreenFlux.Application.Services;
using GreenFlux.Domain.Models;

namespace GreenFlux.Application.Mappers
{
    public interface IChargeStationsDtoMapper
    {
        ChargeStationsDto Map(Group group);
    }

    public class ChargeStationsDtoMapper : IChargeStationsDtoMapper
    {
        private readonly IChargeStationDtoMapper _chargeStationDtoMapper;
        private readonly ILinksService _linksService;

        public ChargeStationsDtoMapper(IChargeStationDtoMapper chargeStationDtoMapper, ILinksService linksService)
        {
            _chargeStationDtoMapper = chargeStationDtoMapper;
            _linksService = linksService;
        }
        
        public ChargeStationsDto Map(Group group)
        {
            var chargeStationsDto = new ChargeStationsDto
            {
                Values = @group.ChargeStations?.Select(_chargeStationDtoMapper.Map) ?? Enumerable.Empty<ChargeStationDto>()
            };

            chargeStationsDto.Links.Add(new LinkDto(RelationShips.Self, _linksService.LinkToChargeStations(@group.Id), Method.Post));
            chargeStationsDto.Links.Add(new LinkDto(RelationShips.Group, _linksService.LinkToGroup(@group.Id), Method.Get));

            return chargeStationsDto;
        }
    }
}
