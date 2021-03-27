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
    public class DeleteConnectorTests
    {
        private readonly Mock<IRepository> _repository = new Mock<IRepository>();
        private readonly Mock<IConnectorsDtoMapper> _connectorsDtoMapper = new Mock<IConnectorsDtoMapper>();
        private readonly Mock<IConnectorDtoMapper> _connectorDtoMapper = new Mock<IConnectorDtoMapper>();
        
        private readonly GreenFlux.Application.Services.ConnectorService _target;

        private readonly short _connectorId = 1;
        private readonly Guid _groupId = Guid.NewGuid();
        private readonly Guid _chargeStationId = Guid.NewGuid();
        private readonly ChargeStation _chargeStation;

        public DeleteConnectorTests()
        {
            var group = new Group(_groupId, "group 1", 20);
            _chargeStation = new ChargeStation(@group, _chargeStationId, "charge station 1", new Dictionary<short, int> {{ _connectorId, 10 }, {2, 5} });
            group.AddChargeStation(_chargeStation);

            _repository.Setup(r => r.GetGroup(_groupId)).Returns(group);

            _target = new GreenFlux.Application.Services.ConnectorService(_repository.Object, _connectorsDtoMapper.Object, _connectorDtoMapper.Object);
        }

        [Fact]
        public void GivenInvalidGroupId_WhenDeletingConnector_NotFoundExceptionShouldBeThrown()
        {
            Assert.Throws<NotFoundException>(() => _target.DeleteConnector(Guid.NewGuid(), _chargeStationId, _connectorId));
        }

        [Fact]
        public void GivenInvalidChargeStationId_WhenDeletingConnector_NotFoundExceptionShouldBeThrown()
        {
            Assert.Throws<NotFoundException>(() => _target.DeleteConnector(_groupId, Guid.NewGuid(), _connectorId));
        }

        [Fact]
        public void GivenInvalidConnectorId_WhenDeletingConnector_NotFoundExceptionShouldBeThrown()
        {
            Assert.Throws<NotFoundException>(() => _target.DeleteConnector(_groupId, _chargeStationId, 3));
        }

        [Fact]
        public void GivenValidIds_WhenDeletingConnector_ConnectorShouldBeBeRemovedFromDomainModel()
        {
            //arrange
            var connectorDtoMapperResult = new ConnectorDto();
            _repository.Setup(m => m.SaveChargeStation(_chargeStation)).Returns(true);

            //act
            _target.DeleteConnector(_groupId, _chargeStationId, _connectorId);

            //assert
            var singleResult = Assert.Single(_chargeStation.ConnectorCapacities);
            Assert.Equal(2, singleResult.id);
            Assert.Equal(5, singleResult.maxCurrentInAmps);
        }

        [Fact]
        public void GivenValidIds_WhenDeletingConnector_ChargeStationShouldBeSavedAfterConnectionIsRemoved()
        {
            //arrange
            var connectorDtoMapperResult = new ConnectorDto();
            _repository.Setup(m => m.SaveChargeStation(It.Is<ChargeStation>(c => c.ConnectorCapacities.Count() == 1))).Returns(true);

            //act
            _target.DeleteConnector(_groupId, _chargeStationId, _connectorId);

            //assert
            _repository.Verify(m => m.SaveChargeStation(It.Is<ChargeStation>(c => c.ConnectorCapacities.Count() == 1)), Times.Once);
        }
    }
}
