using Microsoft.AspNetCore.Mvc;
using System;
using GreenFlux.Application.Exceptions;
using GreenFlux.Application.Models;
using GreenFlux.Application.Services;
using GreenFlux.Application.WriteModels;
using GreenFlux.Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace GreenFlux.Controllers
{
    [ApiController]
    public class ConnectorController : ControllerBase
    {
        private const string ConnectorsTemplate = "groups/{groupIdentifier}/chargestations/{chargeStationIdentifier}/connectors";
        private const string ConnectorTemplate = "groups/{groupIdentifier}/chargestations/{chargeStationIdentifier}/connectors/{connectorIdentifier}";

        private readonly IConnectorService _connectorService;
        private readonly ILinksService _linksService;

        public ConnectorController(IConnectorService connectorService, ILinksService linksService)
        {
            _connectorService = connectorService;
            _linksService = linksService;
        }

        [HttpGet(ConnectorsTemplate, Name = nameof(GetConnectors))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Connectors))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetConnectors(Guid groupIdentifier, Guid chargeStationIdentifier)
        {
            return Ok(_connectorService.GetConnectors(groupIdentifier, chargeStationIdentifier));
        }

        [HttpPost(ConnectorsTemplate, Name = nameof(CreateConnector))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Connector))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(SerializableError))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateConnector(Guid groupIdentifier, Guid chargeStationIdentifier, DtoConnector connector)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var connectorModel = _connectorService.CreateConnector(groupIdentifier, chargeStationIdentifier, connector);
                return Created(_linksService.LinkToConnector(groupIdentifier, chargeStationIdentifier, connectorModel.Identifier), connectorModel);
            }
            catch (DomainException domainException)
            {
                ModelState.AddModelError(domainException.Key, domainException.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPut(ConnectorTemplate, Name = nameof(UpdateConnector))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Connector))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(SerializableError))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateConnector(Guid groupIdentifier, Guid chargeStationIdentifier, short connectorIdentifier, DtoConnector connector)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                return Ok(_connectorService.UpdateConnector(groupIdentifier, chargeStationIdentifier, connectorIdentifier, connector));
            }
            catch (DomainException domainException)
            {
                ModelState.AddModelError(domainException.Key, domainException.Message);
                return BadRequest(ModelState);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet(ConnectorTemplate, Name = nameof(GetConnector))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Connector))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetConnector(Guid groupIdentifier, Guid chargeStationIdentifier, short connectorIdentifier)
        {
            try
            {
                return Ok(_connectorService.GetConnector(groupIdentifier, chargeStationIdentifier, connectorIdentifier));
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete(ConnectorTemplate, Name = nameof(DeleteConnector))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteConnector(Guid groupIdentifier, Guid chargeStationIdentifier, short connectorIdentifier)
        {
            try
            {
                _connectorService.DeleteConnector(groupIdentifier, chargeStationIdentifier, connectorIdentifier);
                return Ok();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }
    }
}
