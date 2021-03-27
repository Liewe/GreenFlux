using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GreenFlux.Application.DtoModels;
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

            var id = Guid.NewGuid();
            var name = "Group 1";
            var capacityInAmps = 123;
            
            _webApplicationFactory.TestDbUtilities.AddGroup(id, name, capacityInAmps);

            //act
            var result = await httpClient.SendAsync<GroupDto>(HttpMethod.Get, $"/groups/{id}");

            //assert
            Assert.Equal(HttpStatusCode.OK, result.Response.StatusCode);
            Assert.Equal(id, result.Content.Id);
            Assert.Equal(name, result.Content.Name);
            Assert.Equal(capacityInAmps, result.Content.CapacityInAmps);
        }

        [Fact]
        public async Task GivenADatabaseNotContainingGroup_WhenGettingGroup_ThenStatusShouldBeNotFound()
        {
            //arrange
            var httpClient = _webApplicationFactory.CreateClient();
            _webApplicationFactory.TestDbUtilities.DeleteAll();
            var id = Guid.NewGuid();

            //act
            var result = await httpClient.SendAsync<GroupDto>(HttpMethod.Get, $"/groups/{id}");

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
                .Select(i => new { Id = Guid.NewGuid(), Name = $"Group {i}", CapacityInAmps = i })
                .ToList();

            testData.ForEach(d => _webApplicationFactory.TestDbUtilities.AddGroup(d.Id, d.Name, d.CapacityInAmps));

            //act
            var result = await httpClient.SendAsync<GroupsDto>(HttpMethod.Get, "/groups");

            //assert
            Assert.Equal(HttpStatusCode.OK, result.Response.StatusCode);
            Assert.Collection(result.Content.Values, testData.Select(testDataItem => new Action<GroupDto>(group =>
            {
                Assert.Equal(testDataItem.Id, group.Id);
                Assert.Equal(testDataItem.Name, group.Name);
                Assert.Equal(testDataItem.CapacityInAmps, group.CapacityInAmps);
            })).ToArray());
        }
        
    }
}
