using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game.Optimization;

public class ParallelStateChecker<T>
{
    public Func<T>[] StateCheckers { get; init; }

    /// <summary>
    /// Runs <see cref="Parallel.ForEach{TSource}(IEnumerable{TSource}, Action{TSource})"/> 
    /// over <see cref="StateCheckers"/> and invoks each element.
    /// </summary>
    /// <returns><see cref="ConcurrentDictionary{T, int}>"/> each element representing a pair with a result of Func invoke and the function's hash</returns>
    public ConcurrentDictionary<T, int> UpdateAllStates()
    {
        ConcurrentDictionary<T, int> result = new();
        Parallel.ForEach(StateCheckers, f => result.TryAdd(f(), f.GetHashCode()));
        return result;
    }
}


public class ParallelStateChecker
{
    public Action[] StateCheckers { get; init; }

    /// <summary>
    /// Runs <see cref="Parallel.ForEach{TSource}(IEnumerable{TSource}, Action)"/> 
    /// over <see cref="StateCheckers"/> and invoks each element.
    /// </summary>
    public void UpdateAllStates()
    {
        Parallel.ForEach(StateCheckers, f => f());
    }
}