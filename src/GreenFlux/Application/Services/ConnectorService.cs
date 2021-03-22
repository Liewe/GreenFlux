using System;
using System.Linq;
using GreenFlux.Application.Exceptions;
using GreenFlux.Application.Mappers;
using GreenFlux.Application.Models;
using GreenFlux.Application.WriteModels;
using GreenFlux.Domain.Exceptions;
using GreenFlux.Infrastructure;

namespace GreenFlux.Application.Services
{
    public interface IConnectorService
    {
        Connectors GetConnectors(Guid groupId, Guid chargeStationId);

        Connector GetConnector(Guid groupId, Guid chargeStationId, short connectorId);

        Connector CreateConnector(Guid groupId, Guid chargeStationId, DtoConnector connector);

        Connector UpdateConnector(Guid groupId, Guid chargeStationId, short connectorId, DtoConnector connector);

        void DeleteConnector(Guid groupId, Guid chargeStationId, short connectorId);
    }

    public class ConnectorService : IConnectorService
    {
        private readonly IRepository _repository;
        private readonly IConnectorsModelMapper _connectorsModelMapper;
        private readonly IConnectorModelMapper _connectorModelMapper;

        public ConnectorService(
            IRepository repository, 
            IConnectorsModelMapper connectorsModelMapper, 
            IConnectorModelMapper connectorModelMapper)
        {
            _repository = repository;
            _connectorsModelMapper = connectorsModelMapper;
            _connectorModelMapper = connectorModelMapper;
        }

        public Connectors GetConnectors(Guid groupId, Guid chargeStationId)
        {
            var chargeStation = GetChargeStation(groupId, chargeStationId);
            return _connectorsModelMapper.Map(chargeStation);
        }
        
        public Connector GetConnector(Guid groupId, Guid chargeStationId, short connectorId)
        {
            var connector = GetConnectorDomainModel(groupId, chargeStationId, connectorId);
            return _connectorModelMapper.Map(connector);
        }

        public Connector CreateConnector(Guid groupId, Guid chargeStationId, DtoConnector connectorDto)
        {
            var chargeStation = GetChargeStation(groupId, chargeStationId);

            var nextAvailableId = chargeStation.GetNextAvailableConnectorId();

            if (nextAvailableId == null)
            {
                throw new DomainException(nameof(Connectors), "No valid id available within the charge station");
            }

            var connector = new Domain.Models.Connector(chargeStation, nextAvailableId.Value)
            {
                MaxCurrentInAmps = connectorDto.MaxCurrentInAmps
            };

            chargeStation.AddConnector(connector);

            if (!_repository.SaveChargeStation(chargeStation))
            {
                throw new Exception("Something went wrong trying to save the charge station");
            }

            return _connectorModelMapper.Map(connector);
        }

        public Connector UpdateConnector(Guid groupId, Guid chargeStationId, short connectorId, DtoConnector connectorDto)
        {
            var connector = GetConnectorDomainModel(groupId, chargeStationId, connectorId);

            connector.MaxCurrentInAmps = connectorDto.MaxCurrentInAmps;

            if (!_repository.SaveChargeStation(connector.ChargeStation))
            {
                throw new Exception("Something went wrong trying to save the charge station");
            }

            return _connectorModelMapper.Map(connector);
        }

        public void DeleteConnector(Guid groupId, Guid chargeStationId, short connectorId)
        {
            var connector = GetConnectorDomainModel(groupId, chargeStationId, connectorId);

            if (connector == null)
            {
                throw new NotFoundException();
            }

            if (!_repository.SaveChargeStation(connector.ChargeStation))
            {
                throw new Exception("Something went wrong trying to save the charge station");
            }
        }

        private Domain.Models.Connector GetConnectorDomainModel(
            Guid groupId, 
            Guid chargeStationId,
            short connectorId)
        {
            var chargeStation = GetChargeStation(groupId, chargeStationId);

            var connector = chargeStation
                .Connectors
                .FirstOrDefault(c => c.Id == connectorId);

            if (connector == null)
            {
                throw new NotFoundException();
            }

            return connector;
        }

        private Domain.Models.ChargeStation GetChargeStation(Guid groupId, Guid chargeStationId)
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
