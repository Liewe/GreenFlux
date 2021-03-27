using System;
using GreenFlux.Application.DtoModels;
using GreenFlux.Application.Exceptions;
using GreenFlux.Application.Mappers;
using GreenFlux.Domain.Models;
using GreenFlux.Infrastructure;

namespace GreenFlux.Application.Services
{
    public interface IGroupService
    {
        GroupsDto GetGroups();

        GroupDto GetGroup(Guid groupId);

        GroupDto CreateGroup(SaveGroupDto group);

        GroupDto UpdateGroup(Guid groupId, SaveGroupDto group);

        void DeleteGroup(Guid groupId);
    }

    public class GroupService : IGroupService
    {
        private readonly IRepository _repository;
        private readonly IGroupsDtoMapper _groupsDtoMapper;
        private readonly IGroupDtoMapper _groupDtoMapper;

        public GroupService(
            IRepository repository, 
            IGroupsDtoMapper groupsDtoMapper, 
            IGroupDtoMapper groupDtoMapper)
        {
            _repository = repository;
            _groupsDtoMapper = groupsDtoMapper;
            _groupDtoMapper = groupDtoMapper;
        }

        public GroupsDto GetGroups()
        {
            var groupsDomainModel = _repository.GetGroups();
            return _groupsDtoMapper.Map(groupsDomainModel);
        }

        public GroupDto GetGroup(Guid groupId)
        {
            var group = _repository.GetGroup(groupId);
            
            if (group == null)
            {
                throw new NotFoundException();
            }

            return _groupDtoMapper.Map(group);
        }

        public GroupDto CreateGroup(SaveGroupDto groupDto)
        {
            var group = new Group(Guid.NewGuid(), groupDto.Name, groupDto.CapacityInAmps);

            if (!_repository.SaveGroup(group))
            {
                throw new Exception("Something went wrong trying to save group");
            }

            return _groupDtoMapper.Map(group);
        }

        public GroupDto UpdateGroup(Guid groupId, SaveGroupDto groupDto)
        {
            var group = _repository.GetGroup(groupId);

            if (group == null)
            {
                throw new NotFoundException();
            }

            group.Name = groupDto.Name;
            group.CapacityInAmps = groupDto.CapacityInAmps;

            if (!_repository.SaveGroup(group))
            {
                throw new Exception("Something went wrong trying to save group");
            }

            return _groupDtoMapper.Map(group);
        }

        public void DeleteGroup(Guid groupId)
        {
            var group = _repository.GetGroup(groupId);

            if (group == null)
            {
                throw new NotFoundException();
            }

            if (!_repository.DeleteGroup(groupId))
            {
                throw new Exception("Something went wrong trying to save group");
            }
        }
    }
}
