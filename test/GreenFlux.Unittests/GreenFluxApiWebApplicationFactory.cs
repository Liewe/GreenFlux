using GreenFlux.Infrastructure.DbContexts;
using GreenFlux.Unittests.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace GreenFlux.Unittests
{
    public class GreenFluxApiWebApplicationFactory : WebApplicationFactory<Startup>
    {
        private readonly TestConnectionFactory _testConnectionFactory = new TestConnectionFactory();

        public ITestDbUtilities TestDbUtilities { get; private set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IConnectionFactory>(_testConnectionFactory);
                services.AddTransient<ITestDbUtilities, TestDbUtilities>();
                TestDbUtilities = services.BuildServiceProvider().GetRequiredService<ITestDbUtilities>();
            });
        }
    }
}
