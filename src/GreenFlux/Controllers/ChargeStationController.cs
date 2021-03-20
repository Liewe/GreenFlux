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
    public class ChargeStationController : ControllerBase
    {
        private const string ChargeStationsTemplate = "groups/{groupIdentifier}/chargestations";
        private const string ChargeStationTemplate = "groups/{groupIdentifier}/chargestations/{chargeStationIdentifier}";

        private readonly IChargeStationService _chargeStationService;
        private readonly ILinksService _linksService;

        public ChargeStationController(IChargeStationService chargeStationService, ILinksService linksService)
        {
            _chargeStationService = chargeStationService;
            _linksService = linksService;
        }

        [HttpGet(ChargeStationsTemplate, Name = nameof(GetChargeStations))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChargeStations))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetChargeStations(Guid groupIdentifier)
        {
            return Ok(_chargeStationService.GetChargeStations(groupIdentifier));
        }

        [HttpPost(ChargeStationsTemplate, Name = nameof(CreateChargeStation))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ChargeStation))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(SerializableError))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateChargeStation(Guid groupIdentifier, DtoChargeStation chargeStation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var chargeStationModel = _chargeStationService.CreateChargeStation(groupIdentifier, chargeStation);
                return Created(_linksService.LinkToChargeStation(groupIdentifier, chargeStationModel.Identifier), chargeStationModel);
            }
            catch (DomainException domainException)
            {
                ModelState.AddModelError(domainException.Key, domainException.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPut(ChargeStationTemplate, Name = nameof(UpdateChargeStation))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChargeStation))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(SerializableError))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateChargeStation(Guid groupIdentifier, Guid chargeStationIdentifier, DtoChargeStation chargeStation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                return Ok(_chargeStationService.UpdateChargeStation(groupIdentifier, chargeStationIdentifier, chargeStation));
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

        [HttpGet(ChargeStationTemplate, Name = nameof(GetChargeStation))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChargeStation))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetChargeStation(Guid groupIdentifier, Guid chargeStationIdentifier)
        {
            try
            {
                return Ok(_chargeStationService.GetChargeStation(groupIdentifier, chargeStationIdentifier));
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete(ChargeStationTemplate, Name = nameof(DeleteChargeStation))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteChargeStation(Guid groupIdentifier, Guid chargeStationIdentifier)
        {
            try
            {
                _chargeStationService.DeleteChargeStation(groupIdentifier, chargeStationIdentifier);
                return Ok();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }
    }
}
