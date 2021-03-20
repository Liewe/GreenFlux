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
        Connectors GetConnectors(Guid groupIdentifier, Guid chargeStationIdentifier);

        Connector GetConnector(Guid groupIdentifier, Guid chargeStationIdentifier, short connectorIdentifier);

        Connector CreateConnector(Guid groupIdentifier, Guid chargeStationIdentifier, DtoConnector connector);

        Connector UpdateConnector(Guid groupIdentifier, Guid chargeStationIdentifier, short connectorIdentifier, DtoConnector connector);

        void DeleteConnector(Guid groupIdentifier, Guid chargeStationIdentifier, short connectorIdentifier);
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

        public Connectors GetConnectors(Guid groupIdentifier, Guid chargeStationIdentifier)
        {
            var chargeStationDomainModel = GetChargeStationDomainModel(groupIdentifier, chargeStationIdentifier);
            return _connectorsModelMapper.Map(chargeStationDomainModel);
        }
        
        public Connector GetConnector(Guid groupIdentifier, Guid chargeStationIdentifier, short connectorIdentifier)
        {
            var connectorDomainModel = GetConnectorDomainModel(groupIdentifier, chargeStationIdentifier, connectorIdentifier);
            return _connectorModelMapper.Map(connectorDomainModel);
        }

        public Connector CreateConnector(Guid groupIdentifier, Guid chargeStationIdentifier, DtoConnector connector)
        {
            var chargeStationDomainModel = GetChargeStationDomainModel(groupIdentifier, chargeStationIdentifier);

            var nextAvailableIdentifier = chargeStationDomainModel.GetNextAvailableConnectorIdentifier();

            if (nextAvailableIdentifier == null)
            {
                throw new DomainException(nameof(Connectors), "No valid identifier available within the charge station");
            }

            var connectorDomainModel = new Domain.Models.Connector(chargeStationDomainModel, nextAvailableIdentifier.Value, connector.MaxCurrentInAmps);
            chargeStationDomainModel.AddConnector(connectorDomainModel);

            if (!_repository.SaveChargeStation(chargeStationDomainModel))
            {
                throw new Exception("Something went wrong trying to save the charge station");
            }

            return _connectorModelMapper.Map(connectorDomainModel);
        }

        public Connector UpdateConnector(Guid groupIdentifier, Guid chargeStationIdentifier, short connectorIdentifier, DtoConnector connector)
        {
            var connectorDomainModel = GetConnectorDomainModel(groupIdentifier, chargeStationIdentifier, connectorIdentifier);

            connectorDomainModel.MaxCurrentInAmps = connector.MaxCurrentInAmps;

            if (!_repository.SaveChargeStation(connectorDomainModel.ChargeStation))
            {
                throw new Exception("Something went wrong trying to save the charge station");
            }

            return _connectorModelMapper.Map(connectorDomainModel);
        }

        public void DeleteConnector(Guid groupIdentifier, Guid chargeStationIdentifier, short connectorIdentifier)
        {
            var connectorDomainModel = GetConnectorDomainModel(groupIdentifier, chargeStationIdentifier, connectorIdentifier);

            if (connectorDomainModel == null)
            {
                throw new NotFoundException();
            }

            if (!_repository.SaveChargeStation(connectorDomainModel.ChargeStation))
            {
                throw new Exception("Something went wrong trying to save the charge station");
            }
        }

        private Domain.Models.Connector GetConnectorDomainModel(
            Guid groupIdentifier, 
            Guid chargeStationIdentifier,
            short connectorIdentifier)
        {
            var chargeStationDomainModel = GetChargeStationDomainModel(groupIdentifier, chargeStationIdentifier);

            var connectorDomainModel = chargeStationDomainModel
                .Connectors
                .FirstOrDefault(c => c.Identifier == connectorIdentifier);

            if (connectorDomainModel == null)
            {
                throw new NotFoundException();
            }

            return connectorDomainModel;
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
