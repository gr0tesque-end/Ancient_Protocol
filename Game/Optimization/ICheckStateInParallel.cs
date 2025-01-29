using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Optimization;

public interface ICheckStateInParallel
{
    public Func<bool>[] Checkings { get; }

    /// <summary>
    /// Runs <see cref="Parallel.ForEach{TSource}(IEnumerable{TSource}, Action{TSource})"/> 
    /// over <see cref="Checkings"/> and invoks each element.
    /// </summary>
    /// <returns><see cref="ConcurrentDictionary{bool, int}>"/> each element representing a pair with a result of Func invoke and the function's hash</returns>
    public ConcurrentDictionary<bool, int> RunAllCheckings()
    {
        ConcurrentDictionary<bool, int> result = new();
        Parallel.ForEach(Checkings, f => result.TryAdd(f(), f.GetHashCode()));
        return result;
    }


}
