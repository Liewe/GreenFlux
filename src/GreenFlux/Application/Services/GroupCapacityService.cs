using System;
using System.Collections.Generic;
using System.Linq;
using GreenFlux.Application.Mappers;
using GreenFlux.Application.Models;
using GreenFlux.Infrastructure;
using Connector = GreenFlux.Domain.Models.Connector;
using Group = GreenFlux.Domain.Models.Group;

namespace GreenFlux.Application.Services
{
    public interface IGroupCapacityService
    {
        Suggestions GetSuggestions(Guid groupId, int capacityNeeded, int maxResults);
    }

    public class GroupCapacityService : IGroupCapacityService
    {
        private readonly ISuggestionsModelMapper _suggestionsModelMapper;
        private readonly IRepository _repository;

        public GroupCapacityService(ISuggestionsModelMapper suggestionsModelMapper, IRepository repository)
        {
            _suggestionsModelMapper = suggestionsModelMapper;
            _repository = repository;
        }

        public Suggestions GetSuggestions(Guid groupId, int capacityNeeded, int maxResults)
        {
            var group = _repository.GetGroup(groupId);

            var connectorSets = FindConnectorsToFreeCapacity(group, capacityNeeded, maxResults, true).ToList();

            if (connectorSets.Any())
            {
                return _suggestionsModelMapper.Map(connectorSets, capacityNeeded, true);
            }

            connectorSets = FindConnectorsToFreeCapacity(group, capacityNeeded, maxResults, false).ToList();
            return _suggestionsModelMapper.Map(connectorSets, capacityNeeded, false);
        }

        private IEnumerable<IEnumerable<Connector>> FindConnectorsToFreeCapacity(Group group, int capacityNeeded, int maxResults, bool exact)
        {
            int? smallestCount = null;

            var connectors = group
                .ChargeStations
                .SelectMany(c => c.Connectors);

            return FindConnectorsToFreeCapacity(connectors, capacityNeeded, exact)
                .TakeWhile(c => c.Count == (smallestCount ??= c.Count))
                .Take(maxResults); 
        }

        /// <summary>
        /// Returns an iterator of all sets of connectors of which MaxCurrentInAmps sums capacityNeed, ordered on set size.
        /// When parameter 'exact' is set to true, all sets which exactly match capacityNeeded are returned.
        /// When parameter 'exact' is set to false, all sets that sum to at least capacityNeeded are returned for which
        /// holds; no connector can be removed from the set without the sum of the set being lower then capacityNeeded.
        /// </summary>
        /// <param name="connectors">All connectors to consider</param>
        /// <param name="capacityNeeded">The sum of MaxCurrentInAmps's we seek</param>
        /// <param name="exact">When true, only sets which exactly match capacityNeeded are returned.</param>
        private IEnumerable<List<Connector>> FindConnectorsToFreeCapacity(IEnumerable<Connector> connectors, int capacityNeeded, bool exact)
        {
            // all connectors ordered from most capacity to least
            var sortedConnectors = connectors.OrderByDescending(c => c.MaxCurrentInAmps).ToArray();
            
            // return all sets of connectors having the smallest count, ordered from smallest total capacity to biggest
            return FindCapacityRecursively(0, capacityNeeded);

            // returns all sets of connectors recursively that sum to at least 'recursiveCapacityNeeded'
            IEnumerable<List<Connector>> FindCapacityRecursively(int currentIndex, int recursiveCapacityNeeded)
            {
                Queue<BatchSetIterator> queue = new Queue<BatchSetIterator>();
                int? biggestSetCount = null;

                for (int index = currentIndex; index < sortedConnectors.Length; index++)
                {
                    var currentConnector = sortedConnectors[index];
                    
                    if (recursiveCapacityNeeded <= currentConnector.MaxCurrentInAmps)
                    {
                        if (recursiveCapacityNeeded == currentConnector.MaxCurrentInAmps || !exact)
                        {
                            yield return new List<Connector> { currentConnector };
                        }
                    }
                    else
                    {
                        var recursiveResults = FindCapacityRecursively(
                            index + 1,
                            recursiveCapacityNeeded - currentConnector.MaxCurrentInAmps);

                        var item = new BatchSetIterator(currentConnector, recursiveResults);

                        biggestSetCount ??= item.NextSetSize;

                        foreach (var result in item.GetNextBatchOfSets(biggestSetCount ?? 0))
                        {
                            yield return result;
                        }

                        if (item.ContainsMoreItems)
                        {
                            queue.Enqueue(item);
                        }
                    }
                }
                
                while (queue.TryDequeue(out var item))
                {
                    if (item.PreviousSetSize == biggestSetCount)
                    {
                        biggestSetCount = item.NextSetSize;
                    }

                    foreach (var result in item.GetNextBatchOfSets(biggestSetCount ?? 0))
                    {
                        yield return result;
                    }
           
                    if (item.ContainsMoreItems)
                    {
                        queue.Enqueue(item);
                    }
                }
            }
        }

        private class BatchSetIterator
        {
            private readonly Connector _connector;
            private readonly IEnumerator<List<Connector>> _enumerator;
            
            public BatchSetIterator(Connector connector, IEnumerable<List<Connector>> enumerable)
            {
                _connector = connector;
                _enumerator = enumerable.GetEnumerator();
                ContainsMoreItems = _enumerator.MoveNext();
            }

            public int? PreviousSetSize{ get; private set; }

            public int? NextSetSize => ContainsMoreItems ? _enumerator?.Current?.Count : null;
       
            public bool ContainsMoreItems { get; private set; }
            
            public IEnumerable<List<Connector>> GetNextBatchOfSets(int currentSetSize)
            {
                if (!ContainsMoreItems)
                {
                    yield break;
                }
                
                while (_enumerator.Current != null && _enumerator.Current.Count == currentSetSize)
                {
                    _enumerator.Current.Add(_connector);
                    yield return _enumerator.Current;
                    ContainsMoreItems = _enumerator.MoveNext();
                }

                PreviousSetSize = currentSetSize;
            }
        }
    }
}
