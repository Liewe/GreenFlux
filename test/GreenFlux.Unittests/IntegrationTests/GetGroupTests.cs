using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GreenFlux.Application.Models;
using GreenFlux.Unittests.Utilities;
using Xunit;

namespace GreenFlux.Unittests.IntegrationTests
{
    public class GetGroupTests : IClassFixture<GreenFluxApiWebApplicationFactory>
    {
        private readonly GreenFluxApiWebApplicationFactory _webApplicationFactory;

        public GetGroupTests(GreenFluxApiWebApplicationFactory webApplicationFactory)
        {
            _webApplicationFactory = webApplicationFactory;
        }

        [Fact]
        public async Task GivenADatabaseContainingGroup_WhenGettingGroup_ThenStatusShouldBeOk_AndContentShouldContainGroup()
        {
            //arrange
            var httpClient = _webApplicationFactory.CreateClient();
            _webApplicationFactory.TestDbUtilities.DeleteAll();

            var identifier = Guid.NewGuid();
            var name = "Group 1";
            var capacityInAmps = 123;
            
            _webApplicationFactory.TestDbUtilities.AddGroup(identifier, name, capacityInAmps);

            //act
            var result = await httpClient.SendAsync<Group>(HttpMethod.Get, $"/groups/{identifier}");

            //assert
            Assert.Equal(HttpStatusCode.OK, result.Response.StatusCode);
            Assert.Equal(identifier, result.Content.Identifier);
            Assert.Equal(name, result.Content.Name);
            Assert.Equal(capacityInAmps, result.Content.CapacityInAmps);
        }

        [Fact]
        public async Task GivenADatabaseNotContainingGroup_WhenGettingGroup_ThenStatusShouldBeNotFound()
        {
            //arrange
            var httpClient = _webApplicationFactory.CreateClient();
            _webApplicationFactory.TestDbUtilities.DeleteAll();
            var identifier = Guid.NewGuid();

            //act
            var result = await httpClient.SendAsync<Group>(HttpMethod.Get, $"/groups/{identifier}");

            //assert
            Assert.Equal(HttpStatusCode.NotFound, result.Response.StatusCode);
        }

        [Fact]
        public async Task GivenADatabaseContainingMultipleGroups_WhenGettingGroups_ThenStatusShouldBeOk_AndContentShouldContainAllGroups()
        {
            //arrange
            var httpClient = _webApplicationFactory.CreateClient();
            _webApplicationFactory.TestDbUtilities.DeleteAll();

            var testData = Enumerable
                .Range(1, 10)
                .Select(i => new { Identifier = Guid.NewGuid(), Name = $"Group {i}", CapacityInAmps = i })
                .ToList();

            testData.ForEach(d => _webApplicationFactory.TestDbUtilities.AddGroup(d.Identifier, d.Name, d.CapacityInAmps));

            //act
            var result = await httpClient.SendAsync<Groups>(HttpMethod.Get, "/groups");

            //assert
            Assert.Equal(HttpStatusCode.OK, result.Response.StatusCode);
            Assert.Collection(result.Content.Values, testData.Select(testDataItem => new Action<Group>(group =>
            {
                Assert.Equal(testDataItem.Identifier, group.Identifier);
                Assert.Equal(testDataItem.Name, group.Name);
                Assert.Equal(testDataItem.CapacityInAmps, group.CapacityInAmps);
            })).ToArray());
        }
        
    }
}
