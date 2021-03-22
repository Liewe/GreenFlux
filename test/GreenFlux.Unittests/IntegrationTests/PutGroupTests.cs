using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GreenFlux.Application.Models;
using GreenFlux.Application.WriteModels;
using GreenFlux.Unittests.Utilities;
using Xunit;

namespace GreenFlux.Unittests.IntegrationTests
{
    public class PutGroupTests : IClassFixture<GreenFluxApiWebApplicationFactory>
    {
        private readonly GreenFluxApiWebApplicationFactory _webApplicationFactory;

        public PutGroupTests(GreenFluxApiWebApplicationFactory webApplicationFactory)
        {
            _webApplicationFactory = webApplicationFactory;
        }
        
        [Fact]
        public async Task GivenADatabaseContainingGroup_WhenPuttingGroup_ThenStatusShouldBeOk_AndContentShouldContainGroup()
        {
            // arrange
            var httpClient = _webApplicationFactory.CreateClient();
            _webApplicationFactory.TestDbUtilities.DeleteAll();

            var identifier = Guid.NewGuid();
            
            _webApplicationFactory.TestDbUtilities.AddGroup(identifier, "Initial name", 10);

            var group = new DtoGroup
            {
                Name = "Changed name",
                CapacityInAmps = 20
            };

            // act
            var result = await httpClient.SendAsync<DtoGroup, Group>(HttpMethod.Put, $"/groups/{identifier}", group);
            
            // assert
            Assert.Equal(HttpStatusCode.OK, result.Response.StatusCode);
            Assert.Equal(group.Name, result.Content.Name);
            Assert.Equal(group.CapacityInAmps, result.Content.CapacityInAmps);
        }

        [Fact]
        public async Task GivenADatabaseContainingGroup_WhenPuttingAFaultyGroup_ThenStatusShouldBeBadRequest()
        {
            // arrange
            var httpClient = _webApplicationFactory.CreateClient();
            _webApplicationFactory.TestDbUtilities.DeleteAll();

            var identifier = Guid.NewGuid();

            _webApplicationFactory.TestDbUtilities.AddGroup(identifier, "Initial name", 10);

            var group = new DtoGroup
            {
                Name = null,
                CapacityInAmps = 0
            };

            // act
            var result = await httpClient.SendAsync<DtoGroup, Group>(HttpMethod.Put, $"/groups/{identifier}", group);

            // assert
            Assert.Equal(HttpStatusCode.BadRequest, result.Response.StatusCode);
        }

        [Fact]
        public async Task GivenADatabaseNotContainingGroup_WhenPuttingGroup_ThenStatusShouldBeNotFound()
        {
            // arrange
            var httpClient = _webApplicationFactory.CreateClient();
            _webApplicationFactory.TestDbUtilities.DeleteAll();

            var identifier = Guid.NewGuid();
            
            var group = new DtoGroup
            {
                Name = "Changed name",
                CapacityInAmps = 20
            };

            // act
            var result = await httpClient.SendAsync<DtoGroup, Group>(HttpMethod.Put, $"/groups/{identifier}", group);

            // assert
            Assert.Equal(HttpStatusCode.NotFound, result.Response.StatusCode);
        }
    }
}
