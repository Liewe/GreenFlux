using System.Collections.Generic;
using System.Linq;
using GreenFlux.Domain.Models;

namespace GreenFlux.Application.Services
{
    public interface IGroupCapacityService
    {
    }

    public class GroupCapacityService : IGroupCapacityService
    {
        public IEnumerable<IEnumerable<Connector>> FindConnectorsToFreeCapacity(Group group, int capacityNeeded, int maxResults, bool exact)
        {
            int? smallestCount = null;
            // all connectors ordered from most capacity to least
            return FindConnectorsToFreeCapacity(group
                .ChargeStations
                .SelectMany(c => c.Connectors), capacityNeeded, exact)
                .TakeWhile(c => c.Count == (smallestCount ??= c.Count))
                .Take(maxResults); 
        }

        public IEnumerable<List<Connector>> FindConnectorsToFreeCapacity(IEnumerable<Connector> connectors, int capacityNeeded, bool exact)
        {
            // all connectors ordered from most capacity to least
            var sortedConnectors = connectors.OrderByDescending(c => c.MaxCurrentInAmps).ToArray();
            
            // return all sets of connectors having the smallest count, ordered from smallest total capacity to biggest
            return FindCapacityRecursively(0, capacityNeeded);

            // returns all sets of connectors recursively  
            IEnumerable<List<Connector>> FindCapacityRecursively(int currentIndex, int capacityNeeded)
            {
                Queue<BatchSetIterator> queue = new Queue<BatchSetIterator>();
                int? biggestSetCount = null;

                for (int index = currentIndex; index < sortedConnectors.Length; index++)
                {
                    var currentConnector = sortedConnectors[index];
                    
                    if (capacityNeeded <= currentConnector.MaxCurrentInAmps)
                    {
                        if (capacityNeeded == currentConnector.MaxCurrentInAmps || !exact)
                        {
                            yield return new List<Connector> { currentConnector };
                        }
                    }
                    else
                    {
                        var recursiveResults = FindCapacityRecursively(
                            index + 1,
                            capacityNeeded - currentConnector.MaxCurrentInAmps);

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

        public class BatchSetIterator
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
