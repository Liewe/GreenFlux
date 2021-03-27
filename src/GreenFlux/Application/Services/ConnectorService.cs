using System;
using System.Linq;
using GreenFlux.Application.DtoModels;
using GreenFlux.Application.Exceptions;
using GreenFlux.Application.Mappers;
using GreenFlux.Domain.Models;
using GreenFlux.Infrastructure;

namespace GreenFlux.Application.Services
{
    public interface IConnectorService
    {
        ConnectorsDto GetConnectors(Guid groupId, Guid chargeStationId);
        ConnectorDto GetConnector(Guid groupId, Guid chargeStationId, short connectorId);
        ConnectorDto CreateConnector(Guid groupId, Guid chargeStationId, SaveConnectorDto connector);
        ConnectorDto UpdateConnector(Guid groupId, Guid chargeStationId, short connectorId, SaveConnectorDto connector);
        void DeleteConnector(Guid groupId, Guid chargeStationId, short connectorId);
    }

    public class ConnectorService : IConnectorService
    {
        private readonly IRepository _repository;
        private readonly IConnectorsDtoMapper _connectorsDtoMapper;
        private readonly IConnectorDtoMapper _connectorDtoMapper;

        public ConnectorService(
            IRepository repository, 
            IConnectorsDtoMapper connectorsDtoMapper, 
            IConnectorDtoMapper connectorDtoMapper)
        {
            _repository = repository;
            _connectorsDtoMapper = connectorsDtoMapper;
            _connectorDtoMapper = connectorDtoMapper;
        }

        public ConnectorsDto GetConnectors(Guid groupId, Guid chargeStationId)
        {
            var chargeStation = GetChargeStation(groupId, chargeStationId);
            return _connectorsDtoMapper.Map(chargeStation);
        }
        
        public ConnectorDto GetConnector(Guid groupId, Guid chargeStationId, short connectorId)
        {
            var chargeStation = GetChargeStation(groupId, chargeStationId);
            var connectorDto = _connectorDtoMapper.Map(chargeStation, connectorId);
            if (connectorDto == null)
            {
                throw new NotFoundException();
            }

            return connectorDto;
        }

        public ConnectorDto CreateConnector(Guid groupId, Guid chargeStationId, SaveConnectorDto connectorDto)
        {
            var chargeStation = GetChargeStation(groupId, chargeStationId);

            var connectorId = chargeStation.AddConnector(connectorDto.MaxCurrentInAmps);

            if (!_repository.SaveChargeStation(chargeStation))
            {
                throw new Exception("Something went wrong trying to save the charge station");
            }

            return _connectorDtoMapper.Map(chargeStation, connectorId);
        }

        public ConnectorDto UpdateConnector(Guid groupId, Guid chargeStationId, short connectorId, SaveConnectorDto connectorDto)
        {
            var chargeStation = GetChargeStation(groupId, chargeStationId);

            if (chargeStation.GetMaxCapacityInAmps(connectorId) == null)
            {
                throw new NotFoundException();
            }

            chargeStation.SetConnectorCapacity(connectorId, connectorDto.MaxCurrentInAmps);
            
            if (!_repository.SaveChargeStation(chargeStation))
            {
                throw new Exception("Something went wrong trying to save the charge station");
            }

            return _connectorDtoMapper.Map(chargeStation, connectorId);
        }

        public void DeleteConnector(Guid groupId, Guid chargeStationId, short connectorId)
        {
            var chargeStation = GetChargeStation(groupId, chargeStationId);
            
            if (!chargeStation.RemoveConnector(connectorId))
            {
                throw new NotFoundException();
            }

            if (!_repository.SaveChargeStation(chargeStation))
            {
                throw new Exception("Something went wrong trying to save the charge station");
            }
        }
        
        private ChargeStation GetChargeStation(Guid groupId, Guid chargeStationId)
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
