using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GreenFlux.Application.Models;
using GreenFlux.Application.WriteModels;
using GreenFlux.Unittests.Utilities;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace GreenFlux.Unittests.IntegrationTests
{
    public class PostGroupTests : IClassFixture<GreenFluxApiWebApplicationFactory>
    {
        private readonly GreenFluxApiWebApplicationFactory _webApplicationFactory;

        public PostGroupTests(GreenFluxApiWebApplicationFactory webApplicationFactory)
        {
            _webApplicationFactory = webApplicationFactory;
        }
        
        [Fact]
        public async Task GivenAnEmptyDB_WhenPostingGroup_ThenStatusShouldBeCreated_AndContentShouldContainGroup()
        {
            // arrange
            var httpClient = _webApplicationFactory.CreateClient();
            _webApplicationFactory.TestDbUtilities.DeleteAll();

            var postGroup = new DtoGroup
            {
                Name = "Post Group",
                CapacityInAmps = 20
            };

            // act
            var result = await httpClient.SendAsync<DtoGroup, Group>(HttpMethod.Post, "/groups", postGroup);

            // assert
            Assert.Equal(HttpStatusCode.Created, result.Response.StatusCode);
            Assert.Equal(postGroup.Name, result.Content.Name);
            Assert.Equal(postGroup.CapacityInAmps, result.Content.CapacityInAmps);
        }

        [Fact]
        public async Task GivenAnEmptyDB_WhenPostingAFaultyGroup_ThenStatusShouldBeBadRequest()
        {
            // arrange
            var httpClient = _webApplicationFactory.CreateClient();
            _webApplicationFactory.TestDbUtilities.DeleteAll();

            var group = new DtoGroup
            {
                Name = null,
                CapacityInAmps = 0
            };

            // act
            var result = await httpClient.SendAsync<DtoGroup, SerializableError>(HttpMethod.Post, "/groups", group);

            // assert
            Assert.Equal(HttpStatusCode.BadRequest, result.Response.StatusCode);
        }
    }
}
