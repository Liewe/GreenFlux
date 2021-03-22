using System;
using System.Linq;
using GreenFlux.Application.Exceptions;
using GreenFlux.Application.Mappers;
using GreenFlux.Application.Models;
using GreenFlux.Application.WriteModels;
using GreenFlux.Infrastructure;
using ChargeStation = GreenFlux.Application.Models.ChargeStation;

namespace GreenFlux.Application.Services
{
    public interface IChargeStationService
    {
        ChargeStations GetChargeStations(Guid groupId);

        ChargeStation GetChargeStation(Guid groupId, Guid chargeStationId);

        ChargeStation CreateChargeStation(Guid groupId, DtoChargeStation chargeStation);

        ChargeStation UpdateChargeStation(Guid groupId, Guid chargeStationId, DtoChargeStation chargeStation);

        void DeleteChargeStation(Guid groupId, Guid chargeStationId);
    }

    public class ChargeStationService : IChargeStationService
    {
        private readonly IRepository _repository;
        private readonly IChargeStationsModelMapper _chargeStationsModelMapper;
        private readonly IChargeStationModelMapper _chargeStationModelMapper;

        public ChargeStationService(
            IRepository repository, 
            IChargeStationsModelMapper chargeStationsModelMapper, 
            IChargeStationModelMapper chargeStationModelMapper)
        {
            _repository = repository;
            _chargeStationsModelMapper = chargeStationsModelMapper;
            _chargeStationModelMapper = chargeStationModelMapper;
        }

        public ChargeStations GetChargeStations(Guid groupId)
        {
            var group = GetGroup(groupId);
            return _chargeStationsModelMapper.Map(group);
        }
        
        public ChargeStation GetChargeStation(Guid groupId, Guid chargeStationId)
        {
            var chargeStation = GetChargeStationDomainModel(groupId, chargeStationId);
            return _chargeStationModelMapper.Map(chargeStation);
        }

        public ChargeStation CreateChargeStation(Guid groupId, DtoChargeStation chargeStationDto)
        {
            var group = GetGroup(groupId);

            var chargeStation = new Domain.Models.ChargeStation(group, Guid.NewGuid(), chargeStationDto.Name);
            group.AddChargeStation(chargeStation);
            chargeStation.SetAllMaxCapacityInAmps(chargeStation.Connectors.Select(c => c.MaxCurrentInAmps));

            if (!_repository.SaveChargeStation(chargeStation))
            {
                throw new Exception("Something went wrong trying to save the charge station");
            }

            return _chargeStationModelMapper.Map(chargeStation);
        }

        public ChargeStation UpdateChargeStation(Guid groupId, Guid chargeStationId, DtoChargeStation chargeStationDto)
        {
            var chargeStation = GetChargeStationDomainModel(groupId, chargeStationId);

            chargeStation.Name = chargeStationDto.Name;
            chargeStation.SetAllMaxCapacityInAmps(chargeStationDto.Connectors.Select(c => c.MaxCurrentInAmps));
            
            if (!_repository.SaveChargeStation(chargeStation))
            {
                throw new Exception("Something went wrong trying to save the charge station");
            }

            return _chargeStationModelMapper.Map(chargeStation);
        }
        
        public void DeleteChargeStation(Guid groupId, Guid chargeStationId)
        {
            var chargeStation = GetChargeStationDomainModel(groupId, chargeStationId);

            if (chargeStation == null)
            {
                throw new NotFoundException();
            }

            chargeStation.Group.RemoveChargeStation(chargeStation);

            if (!_repository.DeleteChargeStation(chargeStationId))
            {
                throw new Exception("Something went wrong trying to save group");
            }
        }

        private Domain.Models.ChargeStation GetChargeStationDomainModel(Guid groupId, Guid chargeStationId)
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

        private Domain.Models.Group GetGroup(Guid groupId)
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
