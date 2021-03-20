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

        Group GetGroup(Guid groupIdentifier);

        Group CreateGroup(DtoGroup group);

        Group UpdateGroup(Guid groupIdentifier, DtoGroup group);

        void DeleteGroup(Guid groupIdentifier);
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

        public Group GetGroup(Guid groupIdentifier)
        {
            var groupDomainModel = _repository.GetGroup(groupIdentifier);
            
            if (groupDomainModel == null)
            {
                throw new NotFoundException();
            }

            return _groupModelMapper.Map(groupDomainModel);
        }

        public Group CreateGroup(DtoGroup group)
        {
            var groupDomainModel = new Domain.Models.Group(Guid.NewGuid(), group.Name, group.CapacityInAmps);

            if (!_repository.SaveGroup(groupDomainModel))
            {
                throw new Exception("Something went wrong trying to save group");
            }

            return _groupModelMapper.Map(groupDomainModel);
        }

        public Group UpdateGroup(Guid groupIdentifier, DtoGroup group)
        {
            var groupDomainModel = _repository.GetGroup(groupIdentifier);

            if (groupDomainModel == null)
            {
                throw new NotFoundException();
            }

            groupDomainModel.Name = group.Name;
            groupDomainModel.CapacityInAmps = group.CapacityInAmps;

            if (!_repository.SaveGroup(groupDomainModel))
            {
                throw new Exception("Something went wrong trying to save group");
            }

            return _groupModelMapper.Map(groupDomainModel);
        }

        public void DeleteGroup(Guid groupIdentifier)
        {
            var groupDomainModel = _repository.GetGroup(groupIdentifier);

            if (groupDomainModel == null)
            {
                throw new NotFoundException();
            }

            if (!_repository.DeleteGroup(groupIdentifier))
            {
                throw new Exception("Something went wrong trying to save group");
            }
        }
    }
}
