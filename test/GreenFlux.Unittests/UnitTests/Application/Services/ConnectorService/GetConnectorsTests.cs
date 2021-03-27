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
    public class GetConnectorsTests
    {
        private readonly Mock<IRepository> _repository = new Mock<IRepository>();
        private readonly Mock<IConnectorsDtoMapper> _connectorsDtoMapper = new Mock<IConnectorsDtoMapper>();
        private readonly Mock<IConnectorDtoMapper> _connectorDtoMapper = new Mock<IConnectorDtoMapper>();
        
        private readonly GreenFlux.Application.Services.ConnectorService _target;

        private readonly short _connectorId = 1;
        private readonly Guid _groupId = Guid.NewGuid();
        private readonly Guid _chargeStationId = Guid.NewGuid();
        private readonly ChargeStation _chargeStation;

        public GetConnectorsTests()
        {
            var group = new Group(_groupId, "group 1", 20);
            _chargeStation = new ChargeStation(@group, _chargeStationId, "charge station 1", new Dictionary<short, int> {{ _connectorId, 10 } });
            group.AddChargeStation(_chargeStation);

            _repository.Setup(r => r.GetGroup(_groupId)).Returns(@group);

            _target = new GreenFlux.Application.Services.ConnectorService(_repository.Object, _connectorsDtoMapper.Object, _connectorDtoMapper.Object);
        }

        [Fact]
        public void GivenInvalidGroupId_WhenGettingConnectors_NotFoundExceptionShouldBeThrown()
        {
            Assert.Throws<NotFoundException>(() => _target.GetConnectors(Guid.NewGuid(), _chargeStationId));
        }

        [Fact]
        public void GivenInvalidChargeStationId_WhenGettingConnectors_NotFoundExceptionShouldBeThrown()
        {
            Assert.Throws<NotFoundException>(() => _target.GetConnectors(_groupId, Guid.NewGuid()));
        }

        [Fact]
        public void GivenValidIds_WhenGettingConnectors_ResultShouldBeConnectorsMapperResult()
        {
            //arrange
            var connectorsDtoMapperResult = new ConnectorsDto();
            _connectorsDtoMapper.Setup(m => m.Map(_chargeStation)).Returns(connectorsDtoMapperResult);

            //act
            var result = _target.GetConnectors(_groupId, _chargeStationId);

            //assert
            Assert.Equal(connectorsDtoMapperResult, result);
        }
    }
}
