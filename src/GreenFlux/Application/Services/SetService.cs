using System;
using System.Collections.Generic;
using System.Linq;
using GreenFlux.Application.Models;

namespace GreenFlux.Application.Services
{
    public interface ISetService
    {
        IEnumerable<IEnumerable<TItem>> FindSmallestSetsForRequiredSum<TItem>(IEnumerable<TItem> set, int sumNeeded, int maxResults, bool exact)
            where TItem : IValue;
    }

    public class SetService: ISetService
    {
        public IEnumerable<IEnumerable<TItem>> FindSmallestSetsForRequiredSum<TItem>(IEnumerable<TItem> set, int sumNeeded, int maxResults, bool exact)
            where TItem : IValue
        {
            int? smallestCount = null;

            return FindSmallestSetsForRequiredSum(set, sumNeeded, exact)
                .TakeWhile(c => c.Count == (smallestCount ??= c.Count))
                .Take(maxResults);
        }


        /// <summary>
        /// Returns an iterator of all sets of itesm of which the value sums the required sum, ordered from smallest to biggest set.
        /// When parameter 'exact' is set to true, all sets which exactly match required sum are returned.
        /// When parameter 'exact' is set to false, all sets that sum to at least required sum are returned for which
        /// holds; no item can be removed from the set without the sum of the set being lower then the required sum.
        /// </summary>
        private IEnumerable<List<TItem>> FindSmallestSetsForRequiredSum<TItem>(IEnumerable<TItem> items, int requiredSum, bool exact)
            where TItem : IValue
        {
            var sortedItems = items.OrderByDescending(c => c.Value).ToArray();
            if (sortedItems.Any(s => s.Value <= 0))
            {
                throw new ArgumentException("Allitems should have a value greater than zero");
            }
            
            return FindSmallestSetsForRequiredSumRecursively(0, requiredSum);

            IEnumerable<List<TItem>> FindSmallestSetsForRequiredSumRecursively(int currentIndex, int recursiveRequiredSum)
            {
                Queue<BatchSetIterator<TItem>> queue = new Queue<BatchSetIterator<TItem>>();
                int? biggestSetCount = null;

                for (int index = currentIndex; index < sortedItems.Length; index++)
                {
                    var currentItem = sortedItems[index];

                    if (recursiveRequiredSum <= currentItem.Value)
                    {
                        if (recursiveRequiredSum == currentItem.Value || !exact)
                        {
                            yield return new List<TItem> { currentItem };
                        }
                    }
                    else
                    {
                        var recursiveResults = FindSmallestSetsForRequiredSumRecursively(
                            index + 1,
                            recursiveRequiredSum - currentItem.Value);

                        var item = new BatchSetIterator<TItem>(currentItem, recursiveResults);

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

        private class BatchSetIterator<TItem>
            where TItem : IValue
        {
            private readonly TItem _item;
            private readonly IEnumerator<List<TItem>> _enumerator;

            public BatchSetIterator(TItem item, IEnumerable<List<TItem>> enumerable)
            {
                _item = item;
                _enumerator = enumerable.GetEnumerator();
                ContainsMoreItems = _enumerator.MoveNext();
            }

            public int? PreviousSetSize { get; private set; }

            public int? NextSetSize => ContainsMoreItems ? _enumerator?.Current?.Count : null;

            public bool ContainsMoreItems { get; private set; }

            public IEnumerable<List<TItem>> GetNextBatchOfSets(int currentSetSize)
            {
                if (!ContainsMoreItems)
                {
                    yield break;
                }

                while (_enumerator.Current != null && _enumerator.Current.Count == currentSetSize)
                {
                    _enumerator.Current.Add(_item);
                    yield return _enumerator.Current;
                    ContainsMoreItems = _enumerator.MoveNext();
                }

                PreviousSetSize = currentSetSize;
            }
        }
    }
}
