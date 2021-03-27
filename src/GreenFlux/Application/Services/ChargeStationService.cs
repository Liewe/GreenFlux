using System;
using System.Linq;
using GreenFlux.Application.DtoModels;
using GreenFlux.Application.Exceptions;
using GreenFlux.Application.Mappers;
using GreenFlux.Domain.Models;
using GreenFlux.Infrastructure;

namespace GreenFlux.Application.Services
{
    public interface IChargeStationService
    {
        ChargeStationsDto GetChargeStations(Guid groupId);
        ChargeStationDto GetChargeStation(Guid groupId, Guid chargeStationId);
        ChargeStationDto CreateChargeStation(Guid groupId, SaveChargeStationDto chargeStation);
        ChargeStationDto UpdateChargeStation(Guid groupId, Guid chargeStationId, SaveChargeStationDto chargeStation);
        void DeleteChargeStation(Guid groupId, Guid chargeStationId);
    }

    public class ChargeStationService : IChargeStationService
    {
        private readonly IRepository _repository;
        private readonly IChargeStationsDtoMapper _chargeStationsDtoMapper;
        private readonly IChargeStationDtoMapper _chargeStationDtoMapper;

        public ChargeStationService(
            IRepository repository, 
            IChargeStationsDtoMapper chargeStationsDtoMapper, 
            IChargeStationDtoMapper chargeStationDtoMapper)
        {
            _repository = repository;
            _chargeStationsDtoMapper = chargeStationsDtoMapper;
            _chargeStationDtoMapper = chargeStationDtoMapper;
        }

        public ChargeStationsDto GetChargeStations(Guid groupId)
        {
            var group = GetGroup(groupId);
            return _chargeStationsDtoMapper.Map(group);
        }
        
        public ChargeStationDto GetChargeStation(Guid groupId, Guid chargeStationId)
        {
            var chargeStation = GetChargeStationDomainModel(groupId, chargeStationId);
            return _chargeStationDtoMapper.Map(chargeStation);
        }

        public ChargeStationDto CreateChargeStation(Guid groupId, SaveChargeStationDto chargeStationDto)
        {
            var group = GetGroup(groupId);

            var connectionCapacities = chargeStationDto.Connectors.Select(c => c.MaxCurrentInAmps);
            var chargeStation = new ChargeStation(group, Guid.NewGuid(), chargeStationDto.Name, connectionCapacities);
            group.AddChargeStation(chargeStation);

            if (!_repository.SaveChargeStation(chargeStation))
            {
                throw new Exception("Something went wrong trying to save the charge station");
            }

            return _chargeStationDtoMapper.Map(chargeStation);
        }

        public ChargeStationDto UpdateChargeStation(Guid groupId, Guid chargeStationId, SaveChargeStationDto chargeStationDto)
        {
            var chargeStation = GetChargeStationDomainModel(groupId, chargeStationId);

            chargeStation.Name = chargeStationDto.Name;
            chargeStation.SetAllConnectorCapacities(chargeStationDto.Connectors.Select(c => c.MaxCurrentInAmps));
            
            if (!_repository.SaveChargeStation(chargeStation))
            {
                throw new Exception("Something went wrong trying to save the charge station");
            }

            return _chargeStationDtoMapper.Map(chargeStation);
        }
        
        public void DeleteChargeStation(Guid groupId, Guid chargeStationId)
        {
            var group = GetGroup(groupId);

            if (!group.RemoveChargeStation(chargeStationId))
            {
                throw new NotFoundException();
            }
            
            if (!_repository.DeleteChargeStation(chargeStationId))
            {
                throw new Exception("Something went wrong trying to save group");
            }
        }

        private ChargeStation GetChargeStationDomainModel(Guid groupId, Guid chargeStationId)
        {
            var group = GetGroup(groupId);

            var chargeStation = group
                .ChargeStations
                .FirstOrDefault(c => c.Id == chargeStationId);

            if (chargeStation == null)
            {
                throw new NotFoundException();
            }

            return chargeStation;

        }

        private Group GetGroup(Guid groupId)
        {
            var group = _repository.GetGroup(groupId);

            if (group == null)
            {
                throw new NotFoundException();
            }

            return group;
        }
    }
}
