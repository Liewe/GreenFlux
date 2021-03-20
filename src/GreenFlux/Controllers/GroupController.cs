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
    public class GroupController : ControllerBase
    {
        private const string GroupsTemplate = "groups";
        private const string GroupTemplate = "groups/{groupIdentifier}";

        private readonly IGroupService _groupService;
        private readonly ILinksService _linksService;

        public GroupController(IGroupService groupService, ILinksService linksService)
        {
            _groupService = groupService;
            _linksService = linksService;
        }

        [HttpGet(GroupsTemplate, Name = nameof(GetGroups))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Groups))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetGroups()
        {
            return Ok(_groupService.GetGroups());
        }

        [HttpPost(GroupsTemplate, Name = nameof(CreateGroup))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Group))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(SerializableError))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateGroup(DtoGroup group)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var groupModel = _groupService.CreateGroup(group);
                return Created(_linksService.LinkToGroup(groupModel.Identifier), groupModel);
            }
            catch (DomainException domainException)
            {
                ModelState.AddModelError(domainException.Key, domainException.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPut(GroupTemplate, Name = nameof(UpdateGroup))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Group))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(SerializableError))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateGroup(Guid groupIdentifier, DtoGroup group)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                return Ok(_groupService.UpdateGroup(groupIdentifier, group));
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

        [HttpGet(GroupTemplate, Name = nameof(GetGroup))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Group))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetGroup(Guid groupIdentifier)
        {
            try
            {
                return Ok(_groupService.GetGroup(groupIdentifier));
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete(GroupTemplate, Name = nameof(DeleteGroup))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteGroup(Guid groupIdentifier)
        {
            try
            {
                _groupService.DeleteGroup(groupIdentifier);
                return Ok();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }
    }
}
