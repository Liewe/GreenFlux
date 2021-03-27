using System;
using System.Collections.Generic;
using System.Linq;
using GreenFlux.Application.DtoModels;
using GreenFlux.Application.Exceptions;
using GreenFlux.Application.Mappers;
using GreenFlux.Infrastructure;
using GreenFlux.Domain.Models;
using Moq;
using Xunit;

namespace GreenFlux.Unittests.UnitTests.Application.Services.ConnectorService
{
    public class UpdateConnectorTests
    {
        private readonly Mock<IRepository> _repository = new Mock<IRepository>();
        private readonly Mock<IConnectorsDtoMapper> _connectorsDtoMapper = new Mock<IConnectorsDtoMapper>();
        private readonly Mock<IConnectorDtoMapper> _connectorDtoMapper = new Mock<IConnectorDtoMapper>();
        
        private readonly GreenFlux.Application.Services.ConnectorService _target;

        private readonly short _connectorId = 1;
        private readonly Guid _groupId = Guid.NewGuid();
        private readonly Guid _chargeStationId = Guid.NewGuid();
        private readonly ChargeStation _chargeStation;

        public UpdateConnectorTests()
        {
            var group = new Group(_groupId, "group 1", 20);
            _chargeStation = new ChargeStation(@group, _chargeStationId, "charge station 1", new Dictionary<short, int> {{ _connectorId, 10 } });
            group.AddChargeStation(_chargeStation);

            _repository.Setup(r => r.GetGroup(_groupId)).Returns(@group);

            _target = new GreenFlux.Application.Services.ConnectorService(_repository.Object, _connectorsDtoMapper.Object, _connectorDtoMapper.Object);
        }

        [Fact]
        public void GivenInvalidGroupId_WhenUpdatingConnector_NotFoundExceptionShouldBeThrown()
        {
            Assert.Throws<NotFoundException>(() => _target.UpdateConnector(Guid.NewGuid(), _chargeStationId, _connectorId, new SaveConnectorDto()));
        }

        [Fact]
        public void GivenInvalidChargeStationId_WhenUpdatingConnector_NotFoundExceptionShouldBeThrown()
        {
            Assert.Throws<NotFoundException>(() => _target.UpdateConnector(_groupId, Guid.NewGuid(), _connectorId, new SaveConnectorDto()));
        }

        [Fact]
        public void GivenInvalidConnectorId_WhenGettingConnector_NotFoundExceptionShouldBeThrown()
        {
            Assert.Throws<NotFoundException>(() => _target.UpdateConnector(_groupId, _chargeStationId, 2, new SaveConnectorDto()));
        }
        
        [Fact]
        public void GivenValidIds_AndValues_WhenUpdatingConnector_ConnectorShouldBeBeUpdatedInDomainModel()
        {
            //arrange
            var saveConnectorDto = new SaveConnectorDto { MaxCurrentInAmps = 5 };
            _repository.Setup(m => m.SaveChargeStation(_chargeStation)).Returns(true);

            //act
            _target.UpdateConnector(_groupId, _chargeStationId, _connectorId, saveConnectorDto);

            //assert
            var singleResult = Assert.Single(_chargeStation.ConnectorCapacities);
            Assert.Equal(5, singleResult.maxCurrentInAmps);
        }

        [Fact]
        public void GivenValidIds_AndValues_WhenUpdatingConnector_ChargeStationShouldBeSavedAfterConnectionIsUpdated()
        {
            //arrange
            var saveConnectorDto = new SaveConnectorDto { MaxCurrentInAmps = 5 };
            _repository.Setup(m => m.SaveChargeStation(It.Is<ChargeStation>(c => c.GetMaxCapacityInAmps(_connectorId) == 5))).Returns(true);

            //act
            _target.UpdateConnector(_groupId, _chargeStationId, _connectorId, saveConnectorDto);

            //assert
            _repository.Verify(m => m.SaveChargeStation(It.Is<ChargeStation>(c => c.GetMaxCapacityInAmps(_connectorId) == 5)), Times.Once);
        }

        [Fact]
        public void GivenValidIds_AndValues_WhenUpdatingConnector_ResultShouldBeConnectorsMapperResult()
        {
            //arrange
            var saveConnectorDto = new SaveConnectorDto { MaxCurrentInAmps = 5};
            var connectorDtoMapperResult = new ConnectorDto();
            _connectorDtoMapper.Setup(m => m.Map(_chargeStation, _connectorId)).Returns(connectorDtoMapperResult);
            _repository.Setup(m => m.SaveChargeStation(_chargeStation)).Returns(true);

            //act
            var result = _target.UpdateConnector(_groupId, _chargeStationId, _connectorId, saveConnectorDto);

            //assert
            Assert.Equal(connectorDtoMapperResult, result);
        }
    }
}
