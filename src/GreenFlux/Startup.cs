using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using GreenFlux.Application.Mappers;
using GreenFlux.Application.Services;
using GreenFlux.Infrastructure;
using GreenFlux.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace GreenFlux
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddTransient<IGroupService, GroupService>();
            services.AddTransient<IChargeStationService, ChargeStationService>();
            services.AddTransient<IConnectorService, ConnectorService>();
            services.AddTransient<IGroupCapacityService, GroupCapacityService>();

            services.AddTransient<ILinksService, LinksService>();

            services.AddTransient<IGroupsModelMapper, GroupsModelMapper>();
            services.AddTransient<IGroupModelMapper, GroupModelMapper>();
            services.AddTransient<IChargeStationsModelMapper, ChargeStationsModelMapper>();
            services.AddTransient<IChargeStationModelMapper, ChargeStationModelMapper>();
            services.AddTransient<IConnectorsModelMapper, ConnectorsModelMapper>();
            services.AddTransient<IConnectorModelMapper, ConnectorModelMapper>();
            services.AddTransient<ISuggestionsModelMapper, SuggestionsModelMapper>();
            services.AddTransient<ISuggestionModelMapper, SuggestionModelMapper>();

            services.AddSingleton<IRepository, Repository>();

            services.AddTransient<IConnectionFactory, ConnectionFactory>();
            services.AddTransient<IGroupDbContext, GroupDbContext>();
            services.AddTransient<IChargeStationDbContext, ChargeStationDbContext>();
            services.AddTransient<IConnectorDbContext, ConnectorDbContext>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GreenFlux", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.ApplicationServices.GetService<IRepository>()?.Initialize();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GreenFlux v1"));
            }
            
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
