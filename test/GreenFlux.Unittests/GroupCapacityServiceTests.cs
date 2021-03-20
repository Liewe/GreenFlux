using System;
using System.Diagnostics;
using System.Linq;
using GreenFlux.Application.Services;
using GreenFlux.Domain;
using GreenFlux.Domain.Models;
using Xunit;
using Xunit.Abstractions;

namespace GreenFlux.Unittests
{
    public class GroupCapacityServiceTests
    {
        private readonly ITestOutputHelper _output;

        public GroupCapacityServiceTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestPerformance()
        {
            GroupCapacityService target = new GroupCapacityService();

            var group = CreateDummy(1000000, 1, 25);

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            foreach (var result in target.FindConnectorsToFreeCapacity(group, 50, 100, true))
            {
                foreach(var connector in result)
                    _output.WriteLine($"Chargestation '{connector.ChargeStation.Name}', connector '{connector.Identifier}' (max {connector.MaxCurrentInAmps} Amps)");
                _output.WriteLine(string.Empty);
            }
            stopWatch.Stop();
            _output.WriteLine($"{stopWatch.ElapsedMilliseconds}ms");
        }

        public Group CreateDummy(int connectorCount, int minAmps, int maxAmps)
        {
            var random = new Random(DateTime.Now.Millisecond);

             var group = new Group(Guid.NewGuid(), "group", 15);
             for (int i = 0; i < connectorCount; i += 5)
             {
                 var chargeStation = new ChargeStation(
                     group, 
                     Guid.NewGuid(), 
                     $"Charge station {i/5}", 
                     Enumerable.Range(0,5).Select(_ => random.Next(minAmps, maxAmps)));

                 group.AddChargeStation(chargeStation);
             }

             return group;
        }
    }
}
