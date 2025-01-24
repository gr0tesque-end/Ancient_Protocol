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
    public ConcurrentDictionary<bool, int> RunAllCheckings()
    {
        ConcurrentDictionary<bool, int> result = new();
        Parallel.ForEach(Checkings, f => result.TryAdd(f(), f.GetHashCode()));
        return result;
    }


}
