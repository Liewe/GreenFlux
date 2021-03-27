using System;
using System.Collections.Generic;
using System.Linq;
using GreenFlux.Application.DtoModels;
using GreenFlux.Application.Mappers;
using GreenFlux.Application.Models;
using GreenFlux.Domain.Models;
using GreenFlux.Infrastructure;

namespace GreenFlux.Application.Services
{
    public interface IGroupCapacityService
    {
        SuggestionsDto GetSuggestions(Guid groupId, int capacityNeeded, int maxResults);
    }

    public class GroupCapacityService : IGroupCapacityService
    {
        private readonly ISuggestionsDtoMapper _suggestionsDtoMapper;
        private readonly IRepository _repository;
        private readonly ISetService _setService;

        public GroupCapacityService(ISuggestionsDtoMapper suggestionsDtoMapper, IRepository repository, ISetService setService)
        {
            _suggestionsDtoMapper = suggestionsDtoMapper;
            _repository = repository;
            _setService = setService;
        }

        public SuggestionsDto GetSuggestions(Guid groupId, int capacityNeeded, int maxResults)
        {
            var group = _repository.GetGroup(groupId);

            var connectorSets = FindConnectorsToFreeCapacity(group, capacityNeeded, maxResults, true).ToList();

            if (connectorSets.Any())
            {
                return _suggestionsDtoMapper.Map(connectorSets, capacityNeeded, true);
            }

            connectorSets = FindConnectorsToFreeCapacity(group, capacityNeeded, maxResults, false).ToList();
            return _suggestionsDtoMapper.Map(connectorSets, capacityNeeded, false);
        }

        private IEnumerable<IEnumerable<ConnectorWrapper>> FindConnectorsToFreeCapacity(Group group, int capacityNeeded, int maxResults, bool exact)
        {
            var connectors = group
                .ChargeStations
                .SelectMany(c => c.ConnectorCapacities.Select(co => new ConnectorWrapper(c, co.id, co.maxCurrentInAmps)));

            return _setService.FindSmallestSetsForRequiredSum(connectors, capacityNeeded, maxResults, exact);
        }
    }
}
