using System;
using System.Collections.Generic;
using GreenFlux.Application.DtoModels;
using GreenFlux.Application.Exceptions;
using GreenFlux.Application.Mappers;
using GreenFlux.Infrastructure;
using GreenFlux.Domain.Models;
using Moq;
using Xunit;

namespace GreenFlux.Unittests.UnitTests.Application.Services.ConnectorService
{
    public class GetConnectorTests
    {
        private readonly Mock<IRepository> _repository = new Mock<IRepository>();
        private readonly Mock<IConnectorsDtoMapper> _connectorsDtoMapper = new Mock<IConnectorsDtoMapper>();
        private readonly Mock<IConnectorDtoMapper> _connectorDtoMapper = new Mock<IConnectorDtoMapper>();
        
        private readonly GreenFlux.Application.Services.ConnectorService _target;

        private readonly short _connectorId = 1;
        private readonly Guid _groupId = Guid.NewGuid();
        private readonly Guid _chargeStationId = Guid.NewGuid();
        private readonly ChargeStation _chargeStation;

        public GetConnectorTests()
        {
            var group = new Group(_groupId, "group 1", 20);
            _chargeStation = new ChargeStation(@group, _chargeStationId, "charge station 1", new Dictionary<short, int> {{ _connectorId, 10 } });
            group.AddChargeStation(_chargeStation);

            _repository.Setup(r => r.GetGroup(_groupId)).Returns(@group);

            _target = new GreenFlux.Application.Services.ConnectorService(_repository.Object, _connectorsDtoMapper.Object, _connectorDtoMapper.Object);
        }

        [Fact]
        public void GivenInvalidGroupId_WhenGettingConnector_NotFoundExceptionShouldBeThrown()
        {
            Assert.Throws<NotFoundException>(() => _target.GetConnector(Guid.NewGuid(), _chargeStationId, _connectorId));
        }

        [Fact]
        public void GivenInvalidChargeStationId_WhenGettingConnector_NotFoundExceptionShouldBeThrown()
        {
            Assert.Throws<NotFoundException>(() => _target.GetConnector(_groupId, Guid.NewGuid(), _connectorId));
        }

        [Fact]
        public void GivenInvalidConnectorId_WhenGettingConnector_NotFoundExceptionShouldBeThrown()
        {
            Assert.Throws<NotFoundException>(() => _target.GetConnector(_groupId, _chargeStationId, 2));
        }

        [Fact]
        public void GivenValidIds_WhenGettingConnector_ResultShouldBeConnectorsMapperResult()
        {
            //arrange
            var connectorDtoMapperResult = new ConnectorDto();
            _connectorDtoMapper.Setup(m => m.Map(_chargeStation, _connectorId)).Returns(connectorDtoMapperResult);

            //act
            var result = _target.GetConnector(_groupId, _chargeStationId, _connectorId);

            //assert
            Assert.Equal(connectorDtoMapperResult, result);
        }
    }
}
