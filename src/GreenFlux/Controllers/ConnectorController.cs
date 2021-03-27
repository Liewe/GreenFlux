using Microsoft.AspNetCore.Mvc;
using System;
using GreenFlux.Application.DtoModels;
using GreenFlux.Application.Exceptions;
using GreenFlux.Application.Services;
using GreenFlux.Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace GreenFlux.Controllers
{
    [ApiController]
    public class ConnectorController : ControllerBase
    {
        private const string ConnectorsTemplate = "groups/{groupId}/chargestations/{chargeStationId}/connectors";
        private const string ConnectorTemplate = "groups/{groupId}/chargestations/{chargeStationId}/connectors/{connectorId}";

        private readonly IConnectorService _connectorService;
        private readonly IGroupCapacityService _groupCapacityService;
        private readonly ILinksService _linksService;

        public ConnectorController(IConnectorService connectorService, ILinksService linksService, IGroupCapacityService groupCapacityService)
        {
            _connectorService = connectorService;
            _linksService = linksService;
            _groupCapacityService = groupCapacityService;
        }

        [HttpGet(ConnectorsTemplate, Name = nameof(GetConnectors))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ConnectorsDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetConnectors(Guid groupId, Guid chargeStationId)
        {
            return Ok(_connectorService.GetConnectors(groupId, chargeStationId));
        }

        [HttpPost(ConnectorsTemplate, Name = nameof(CreateConnector))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ConnectorDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(SerializableError))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(SuggestionsDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateConnector(Guid groupId, Guid chargeStationId, SaveConnectorDto connector)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var connectorModel = _connectorService.CreateConnector(groupId, chargeStationId, connector);
                return Created(_linksService.LinkToConnector(groupId, chargeStationId, connectorModel.Id), connectorModel);
            }
            catch (NotEnoughCapacityException notEnoughCapicityException)
            {
                return BadRequest(_groupCapacityService.GetSuggestions(groupId, notEnoughCapicityException.CapacityNeeded, 50));
            }
            catch (DomainException domainException)
            {
                ModelState.AddModelError("", domainException.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPut(ConnectorTemplate, Name = nameof(UpdateConnector))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ConnectorDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(SerializableError))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(SuggestionsDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateConnector(Guid groupId, Guid chargeStationId, short connectorId, SaveConnectorDto connector)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                return Ok(_connectorService.UpdateConnector(groupId, chargeStationId, connectorId, connector));
            }
            catch (NotEnoughCapacityException notEnoughCapicityException)
            {
                return BadRequest(_groupCapacityService.GetSuggestions(groupId, notEnoughCapicityException.CapacityNeeded, 50));
            }
            catch (DomainException domainException)
            {
                ModelState.AddModelError("", domainException.Message);
                return BadRequest(ModelState);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet(ConnectorTemplate, Name = nameof(GetConnector))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ConnectorDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetConnector(Guid groupId, Guid chargeStationId, short connectorId)
        {
            try
            {
                return Ok(_connectorService.GetConnector(groupId, chargeStationId, connectorId));
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
        public IActionResult DeleteConnector(Guid groupId, Guid chargeStationId, short connectorId)
        {
            try
            {
                _connectorService.DeleteConnector(groupId, chargeStationId, connectorId);
                return Ok();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }
    }
}
