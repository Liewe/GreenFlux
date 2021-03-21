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
        ChargeStations GetChargeStations(Guid groupIdentifier);

        ChargeStation GetChargeStation(Guid groupIdentifier, Guid chargeStationIdentifier);

        ChargeStation CreateChargeStation(Guid groupIdentifier, DtoChargeStation chargeStation);

        ChargeStation UpdateChargeStation(Guid groupIdentifier, Guid chargeStationIdentifier, DtoChargeStation chargeStation);

        void DeleteChargeStation(Guid groupIdentifier, Guid chargeStationIdentifier);
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

        public ChargeStations GetChargeStations(Guid groupIdentifier)
        {
            var groupDomainModel = GetGroupDomainModel(groupIdentifier);
            return _chargeStationsModelMapper.Map(groupDomainModel);
        }
        
        public ChargeStation GetChargeStation(Guid groupIdentifier, Guid chargeStationIdentifier)
        {
            var chargeStationDomainModel = GetChargeStationDomainModel(groupIdentifier, chargeStationIdentifier);
            return _chargeStationModelMapper.Map(chargeStationDomainModel);
        }

        public ChargeStation CreateChargeStation(Guid groupIdentifier, DtoChargeStation chargeStation)
        {
            var groupDomainModel = GetGroupDomainModel(groupIdentifier);

            var chargeStationDomainModel = new Domain.Models.ChargeStation(groupDomainModel, Guid.NewGuid(), chargeStation.Name);
            groupDomainModel.AddChargeStation(chargeStationDomainModel);
            chargeStationDomainModel.SetAllMaxCapacityInAmps(chargeStation.Connectors.Select(c => c.MaxCurrentInAmps));

            if (!_repository.SaveChargeStation(chargeStationDomainModel))
            {
                throw new Exception("Something went wrong trying to save the charge station");
            }

            return _chargeStationModelMapper.Map(chargeStationDomainModel);
        }

        public ChargeStation UpdateChargeStation(Guid groupIdentifier, Guid chargeStationIdentifier, DtoChargeStation chargeStation)
        {
            var chargeStationDomainModel = GetChargeStationDomainModel(groupIdentifier, chargeStationIdentifier);

            chargeStationDomainModel.Name = chargeStation.Name;
            chargeStationDomainModel.SetAllMaxCapacityInAmps(chargeStation.Connectors.Select(c => c.MaxCurrentInAmps));
            
            if (!_repository.SaveChargeStation(chargeStationDomainModel))
            {
                throw new Exception("Something went wrong trying to save the charge station");
            }

            return _chargeStationModelMapper.Map(chargeStationDomainModel);
        }
        
        public void DeleteChargeStation(Guid groupIdentifier, Guid chargeStationIdentifier)
        {
            var chargeStationDomainModel = GetChargeStationDomainModel(groupIdentifier, chargeStationIdentifier);

            if (chargeStationDomainModel == null)
            {
                throw new NotFoundException();
            }

            if (!_repository.DeleteChargeStation(chargeStationIdentifier))
            {
                throw new Exception("Something went wrong trying to save group");
            }
        }

        private Domain.Models.ChargeStation GetChargeStationDomainModel(Guid groupIdentifier, Guid chargeStationIdentifier)
        {
            var groupDomainModel = GetGroupDomainModel(groupIdentifier);

            var chargeStationDomainModel = groupDomainModel
                .ChargeStations
                .FirstOrDefault(c => c.Identifier == chargeStationIdentifier);

            if (chargeStationDomainModel == null)
            {
                throw new NotFoundException();
            }

            return chargeStationDomainModel;

        }

        private Domain.Models.Group GetGroupDomainModel(Guid groupIdentifier)
        {
            var groupDomainModel = _repository.GetGroup(groupIdentifier);

            if (groupDomainModel == null)
            {
                throw new NotFoundException();
            }

            return groupDomainModel;
        }
    }
}
