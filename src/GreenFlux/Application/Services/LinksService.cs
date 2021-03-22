using System;
using GreenFlux.Controllers;
using GreenFlux.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace GreenFlux.Application.Services
{ 
    public interface ILinksService
    {
        string LinkToGroups();
        string LinkToGroup(Guid groupId);
        string LinkToChargeStation(ChargeStation chargeStation);
        string LinkToChargeStation(Guid groupId, Guid id);
        string LinkToChargeStations(Guid groupId);
        string LinkToConnectors(Connector connector);
        string LinkToConnectors(ChargeStation chargeStation);
        string LinkToConnector(Connector connector);
        string LinkToConnector(Guid groupId, Guid chargeStationId, short id);
    }

    public class LinksService : ILinksService
    {
        private readonly IUrlHelper _urlHelper;

        public LinksService(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor)
        {
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
        }

        public string LinkToGroups() => 
            _urlHelper.Link(nameof(GroupController.GetGroups), new { });

        public string LinkToGroup(Guid groupId) => 
            _urlHelper.Link(nameof(GroupController.GetGroup), new { groupId });
        
        public string LinkToChargeStations(Guid groupId) => 
            _urlHelper.Link(nameof(ChargeStationController.GetChargeStations), new { groupId });

        public string LinkToChargeStation(ChargeStation chargeStation) =>
            LinkToChargeStation(chargeStation.Group.Id, chargeStation.Id);
        
        public string LinkToChargeStation(Guid groupId, Guid chargeStationId) =>
            _urlHelper.Link(nameof(ChargeStationController.GetChargeStation), new { groupId, chargeStationId });

        public string LinkToConnectors(Connector connector) =>
            LinkToConnectors(connector.ChargeStation);

        public string LinkToConnectors(ChargeStation chargeStation) => LinkToConnectors(
            chargeStation.Group.Id,
            chargeStation.Id);

        public string LinkToConnectors(Guid groupId, Guid chargeStationId) =>
            _urlHelper.Link(nameof(ConnectorController.GetConnectors), new { groupId, chargeStationId });

        public string LinkToConnector(Connector connector) => LinkToConnector(
            connector.ChargeStation.Group.Id,
            connector.ChargeStation.Id, 
            connector.Id);

        public string LinkToConnector(Guid groupId, Guid chargeStationId, short connectorId) =>
            _urlHelper.Link(nameof(ConnectorController.GetConnector), new { groupId, chargeStationId, connectorId });
    }
}
