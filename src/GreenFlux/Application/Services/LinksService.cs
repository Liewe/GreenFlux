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
        string LinkToGroup(Guid groupIdentifier);
        string LinkToChargeStation(ChargeStation chargeStation);
        string LinkToChargeStation(Guid groupIdentifier, Guid identifier);
        string LinkToChargeStations(Guid groupIdentifier);
        string LinkToConnectors(Connector connectorDomainModel);
        string LinkToConnectors(ChargeStation chargeStationDomainModel);
        string LinkToConnector(Connector connectorDomainModel);
        string LinkToConnector(Guid groupIdentifier, Guid chargeStationIdentifier, short identifier);
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

        public string LinkToGroup(Guid groupIdentifier) => 
            _urlHelper.Link(nameof(GroupController.GetGroup), new { groupIdentifier });
        
        public string LinkToChargeStations(Guid groupIdentifier) => 
            _urlHelper.Link(nameof(ChargeStationController.GetChargeStations), new { groupIdentifier });

        public string LinkToChargeStation(ChargeStation chargeStation) =>
            LinkToChargeStation(chargeStation.Group.Identifier, chargeStation.Identifier);
        
        public string LinkToChargeStation(Guid groupIdentifier, Guid chargeStationIdentifier) =>
            _urlHelper.Link(nameof(ChargeStationController.GetChargeStation), new { groupIdentifier, chargeStationIdentifier });

        public string LinkToConnectors(Connector connectorDomainModel) =>
            LinkToConnectors(connectorDomainModel.ChargeStation);

        public string LinkToConnectors(ChargeStation chargeStationDomainModel) => LinkToConnectors(
            chargeStationDomainModel.Group.Identifier,
            chargeStationDomainModel.Identifier);

        public string LinkToConnectors(Guid groupIdentifier, Guid chargeStationIdentifier) =>
            _urlHelper.Link(nameof(ConnectorController.GetConnectors), new { groupIdentifier, chargeStationIdentifier });

        public string LinkToConnector(Connector connectorDomainModel) => LinkToConnector(
            connectorDomainModel.ChargeStation.Group.Identifier,
            connectorDomainModel.ChargeStation.Identifier, 
            connectorDomainModel.Identifier);

        public string LinkToConnector(Guid groupIdentifier, Guid chargeStationIdentifier, short connectorIdentifier) =>
            _urlHelper.Link(nameof(ConnectorController.GetConnector), new { groupIdentifier, chargeStationIdentifier, connectorIdentifier });

    }
}
