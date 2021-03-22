using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GreenFlux.Unittests.Utilities;
using Xunit;

namespace GreenFlux.Unittests.IntegrationTests
{
    public class DeleteGroupTests : IClassFixture<GreenFluxApiWebApplicationFactory>
    {
        private readonly GreenFluxApiWebApplicationFactory _webApplicationFactory;

        public DeleteGroupTests(GreenFluxApiWebApplicationFactory webApplicationFactory)
        {
            _webApplicationFactory = webApplicationFactory;
        }

        [Fact]
        public async Task GivenADatabaseContainingGroup_WhenDeletingGroup_ThenStatusShouldBeOk()
        {
            //arrange
            var httpClient = _webApplicationFactory.CreateClient();
            _webApplicationFactory.TestDbUtilities.DeleteAll();

            var identifier = Guid.NewGuid();
            var name = "Group 1";
            var capacityInAmps = 123;

            _webApplicationFactory.TestDbUtilities.AddGroup(identifier, name, capacityInAmps);

            //act
            var result = await httpClient.SendAsync(HttpMethod.Delete, $"/groups/{identifier}");

            //assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task GivenADatabaseNotContainingGroup_WhenDeletingGroup_ThenStatusShouldBeNotFound()
        {
            //arrange
            var httpClient = _webApplicationFactory.CreateClient();
            _webApplicationFactory.TestDbUtilities.DeleteAll();
            var identifier = Guid.NewGuid();

            //act
            var result = await httpClient.SendAsync(HttpMethod.Get, $"/groups/{identifier}");

            //assert
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }
    }
}
