using System;
using GreenFlux.Application.Exceptions;
using GreenFlux.Application.Mappers;
using GreenFlux.Application.Models;
using GreenFlux.Application.WriteModels;
using GreenFlux.Infrastructure;

namespace GreenFlux.Application.Services
{
    public interface IGroupService
    {
        Groups GetGroups();

        Group GetGroup(Guid groupId);

        Group CreateGroup(DtoGroup group);

        Group UpdateGroup(Guid groupId, DtoGroup group);

        void DeleteGroup(Guid groupId);
    }

    public class GroupService : IGroupService
    {
        private readonly IRepository _repository;
        private readonly IGroupsModelMapper _groupsModelMapper;
        private readonly IGroupModelMapper _groupModelMapper;

        public GroupService(
            IRepository repository, 
            IGroupsModelMapper groupsModelMapper, 
            IGroupModelMapper groupModelMapper)
        {
            _repository = repository;
            _groupsModelMapper = groupsModelMapper;
            _groupModelMapper = groupModelMapper;
        }

        public Groups GetGroups()
        {
            var groupsDomainModel = _repository.GetGroups();
            return _groupsModelMapper.Map(groupsDomainModel);
        }

        public Group GetGroup(Guid groupId)
        {
            var group = _repository.GetGroup(groupId);
            
            if (group == null)
            {
                throw new NotFoundException();
            }

            return _groupModelMapper.Map(group);
        }

        public Group CreateGroup(DtoGroup groupDto)
        {
            var group = new Domain.Models.Group(Guid.NewGuid(), groupDto.Name, groupDto.CapacityInAmps);

            if (!_repository.SaveGroup(group))
            {
                throw new Exception("Something went wrong trying to save group");
            }

            return _groupModelMapper.Map(group);
        }

        public Group UpdateGroup(Guid groupId, DtoGroup groupDto)
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
