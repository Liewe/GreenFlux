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
    public class ChargeStationController : ControllerBase
    {
        private const string ChargeStationsTemplate = "groups/{groupId}/chargestations";
        private const string ChargeStationTemplate = "groups/{groupId}/chargestations/{chargeStationId}";

        private readonly IChargeStationService _chargeStationService;
        private readonly IGroupCapacityService _groupCapacityService;
        private readonly ILinksService _linksService;

        public ChargeStationController(IChargeStationService chargeStationService, ILinksService linksService, IGroupCapacityService groupCapacityService)
        {
            _chargeStationService = chargeStationService;
            _linksService = linksService;
            _groupCapacityService = groupCapacityService;
        }

        [HttpGet(ChargeStationsTemplate, Name = nameof(GetChargeStations))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChargeStationsDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetChargeStations(Guid groupId)
        {
            return Ok(_chargeStationService.GetChargeStations(groupId));
        }

        [HttpPost(ChargeStationsTemplate, Name = nameof(CreateChargeStation))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ChargeStationDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(SerializableError))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(SuggestionsDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateChargeStation(Guid groupId, SaveChargeStationDto chargeStation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var chargeStationModel = _chargeStationService.CreateChargeStation(groupId, chargeStation);
                return Created(_linksService.LinkToChargeStation(groupId, chargeStationModel.Id), chargeStationModel);
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

        [HttpPut(ChargeStationTemplate, Name = nameof(UpdateChargeStation))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChargeStationDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(SerializableError))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(SuggestionsDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateChargeStation(Guid groupId, Guid chargeStationId, SaveChargeStationDto chargeStation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                return Ok(_chargeStationService.UpdateChargeStation(groupId, chargeStationId, chargeStation));
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

        [HttpGet(ChargeStationTemplate, Name = nameof(GetChargeStation))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChargeStationDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetChargeStation(Guid groupId, Guid chargeStationId)
        {
            try
            {
                return Ok(_chargeStationService.GetChargeStation(groupId, chargeStationId));
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
        public IActionResult DeleteChargeStation(Guid groupId, Guid chargeStationId)
        {
            try
            {
                _chargeStationService.DeleteChargeStation(groupId, chargeStationId);
                return Ok();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }
    }
}
