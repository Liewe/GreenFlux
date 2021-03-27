using System;
using GreenFlux.Application.DtoModels;
using GreenFlux.Application.Exceptions;
using GreenFlux.Application.Mappers;
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
        private readonly IGroupsDtoMapper _groupsModelMapper;
        private readonly IGroupDtoMapper _groupModelMapper;

        public GroupService(
            IRepository repository, 
            IGroupsDtoMapper groupsModelMapper, 
            IGroupDtoMapper groupModelMapper)
        {
            _repository = repository;
            _groupsModelMapper = groupsModelMapper;
            _groupModelMapper = groupModelMapper;
        }

        public GroupsDto GetGroups()
        {
            var groupsDomainModel = _repository.GetGroups();
            return _groupsModelMapper.Map(groupsDomainModel);
        }

        public GroupDto GetGroup(Guid groupId)
        {
            var group = _repository.GetGroup(groupId);
            
            if (group == null)
            {
                throw new NotFoundException();
            }

            return _groupModelMapper.Map(group);
        }

        public GroupDto CreateGroup(SaveGroupDto groupDto)
        {
            var group = new Domain.Models.Group(Guid.NewGuid(), groupDto.Name, groupDto.CapacityInAmps);

            if (!_repository.SaveGroup(group))
            {
                throw new Exception("Something went wrong trying to save group");
            }

            return _groupModelMapper.Map(group);
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

            return _groupModelMapper.Map(group);
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
