using GreenFlux.Application.Constants;
using GreenFlux.Application.Models;
using GreenFlux.Application.Services;

namespace GreenFlux.Application.Mappers
{
    public interface IConnectorModelMapper
    {
        Connector Map(Domain.Models.Connector connectorDomainModel);
    }

    public class ConnectorModelMapper : IConnectorModelMapper
    {
        private readonly ILinksService _linksService;

        public ConnectorModelMapper(ILinksService linksService)
        {
            _linksService = linksService;
        }

        public Connector Map(Domain.Models.Connector connectorDomainModel)
        {
            var connector = new Connector
            {
                Identifier = connectorDomainModel.Identifier,
                MaxCurrentInAmps = connectorDomainModel.MaxCurrentInAmps
            };

            connector.Links.Add(new Link(RelationShips.All, _linksService.LinkToConnectors(connectorDomainModel), Method.Get));
            connector.Links.Add(new Link(RelationShips.Self, _linksService.LinkToConnector(connectorDomainModel), Method.Get));
            connector.Links.Add(new Link(RelationShips.Self, _linksService.LinkToConnector(connectorDomainModel), Method.Put));
            connector.Links.Add(new Link(RelationShips.Self, _linksService.LinkToConnector(connectorDomainModel), Method.Delete));

            return connector;
        }
    }
}
