using System;
using System.Collections.Generic;
using GreenFlux.Domain.Exceptions;
using GreenFlux.Domain.Models;
using Xunit;

namespace GreenFlux.Unittests.UnitTests.Domain.Models
{
    public class ChargeStationTests
    {
        [Fact]
        public void GivenAGroupWithoutEnoughCapacity_WhenConstructingChargeStationWithAnonymousConnectors_NotEnoughCapacityExceptionShouldBeThrown()
        {
            // arrange
            Group group = new Group(Guid.NewGuid(), "group 1", 20);
            var connectorCapacities = new[] {10, 20};

            // act + assert
            Assert.Throws<NotEnoughCapacityException>(() => new ChargeStation(group, Guid.NewGuid(), "charge station 1", connectorCapacities));
        }

        [Fact]
        public void GivenAGroupWithEnoughCapacity_WhenConstructingChargeStationWithAnonymousConnectors_ResultShouldContainConnectors()
        {
            // arrange
            Group group = new Group(Guid.NewGuid(), "group 1", 30);
            var connectorCapacities = new[] { 10, 20 };

            // act
            var result = new ChargeStation(group, Guid.NewGuid(), "charge station 1", connectorCapacities);

            // assert
            Assert.Collection(result.ConnectorCapacities,
                connector1 =>
                {
                    Assert.Equal(1, connector1.id);
                    Assert.Equal(10, connector1.maxCurrentInAmps);
                },
                connector2 =>
                {
                    Assert.Equal(2, connector2.id);
                    Assert.Equal(20, connector2.maxCurrentInAmps);
                });
        }

        [Fact]
        public void GivenAGroupWithoutEnoughCapacity_WhenConstructingChargeStationWithSpecificConnectors_NotEnoughCapacityExceptionShouldBeThrown()
        {
            // arrange
            Group group = new Group(Guid.NewGuid(), "group 1", 20);
            var connectorCapacities = new Dictionary<short, int> {{1, 10}, {2, 20}};
            
            // act + assert
            Assert.Throws<NotEnoughCapacityException>(() => new ChargeStation(group, Guid.NewGuid(), "charge station 1", connectorCapacities));
        }

        [Fact]
        public void GivenAGroupWithEnoughCapacity_WhenConstructingChargeStationWithSpecificConnectors_ResultShouldContainConnectors()
        {
            // arrange
            Group group = new Group(Guid.NewGuid(), "group 1", 30);
            var connectorCapacities = new Dictionary<short, int> { { 1, 10 }, { 2, 20 } };

            // act
            var result = new ChargeStation(group, Guid.NewGuid(), "charge station 1", connectorCapacities);

            // assert
            Assert.Collection(result.ConnectorCapacities,
                connector1 =>
                {
                    Assert.Equal(1, connector1.id);
                    Assert.Equal(10, connector1.maxCurrentInAmps);
                },
                connector2 =>
                {
                    Assert.Equal(2, connector2.id);
                    Assert.Equal(20, connector2.maxCurrentInAmps);
                });
        }

        [Fact]
        public void GivenAChargeStationWithMaxAmountOfConnectors_WhenAddingConnector_DomainExceptionShouldBeThrown()
        {
            // arrange
            Group group = new Group(Guid.NewGuid(), "group 1", 30);
            ChargeStation charchStation = new ChargeStation(group, Guid.NewGuid(), "charge station 1", new[] {1, 2, 3, 4, 5});

            // act + assert
            Assert.Throws<DomainException>(() => charchStation.AddConnector(3));
        }
    }
}
