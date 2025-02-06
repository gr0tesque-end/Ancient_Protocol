using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Misc;

public static class DictionaryCaster
{
    public static Dictionary<TResult1, TResult2> Cast<T1, T2, TResult1, TResult2>(
        Dictionary<T1, T2> dict,
        Func<KeyValuePair<T1, T2>, KeyValuePair<TResult1, TResult2>> castingPolicy)
    {
        Dictionary<TResult1, TResult2> res = [];

        foreach (var item in dict)
        {
            var t = castingPolicy(item);
            res.Add(t.Key, t.Value);
        }

        return res;
    }

    public static Dictionary<TResult1, T2> CastItem<T1, T2, TResult1>(
        Dictionary<T1, T2> dict,
        Func<KeyValuePair<T1, T2>, KeyValuePair<TResult1, T2>> castingPolicy)
    {
        Dictionary<TResult1, T2> res = [];

        foreach (var item in dict)
        {
            var t = castingPolicy(item);
            res.Add(t.Key, t.Value);
        }

        return res;
    }

    public static Dictionary<T1, TResult2> CastVal<T1, T2, TResult2>(
       Dictionary<T1, T2> dict,
       Func<KeyValuePair<T1, T2>, KeyValuePair<T1, TResult2>> castingPolicy)
    {
        Dictionary<T1, TResult2> res = [];

        foreach (var item in dict)
        {
            var t = castingPolicy(item);
            res.Add(t.Key, t.Value);
        }

        return res;
    }

}
