using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Game.Misc;

#nullable enable
internal struct Expression(int currentVal, int minVal, int maxVal, int actionVal)
{
    public int CurrentVal = currentVal; 
    public int MinVal = minVal;
    public int MaxVal = maxVal;
    public int ActionVal = actionVal;
};

/// <summary>
/// Dynamically changes filepaths based on; <br/>
/// Same 
/// </summary>
internal class StateBasedPath
{
    private string _localBasePath;

    private readonly ConcurrentBag<Expression> _expressions = [];

    /// <summary>
    /// Syntax: ["number1"-"number2"\"increment_value"]<br/>
    /// Note: there's no value check on number values;<br/>
    /// <br/>Negative values are possible, as long as properly set up
    /// </summary>
    /// <param name="BasePath"></param>
    public StateBasedPath(string BasePath)
    {
        _localBasePath = BasePath;

        DoActionOnExp((expIndex, boundaryIndex) =>
        {
            Task.Run(
                    () => Parse(
                        (string)_localBasePath
                            .SkipLast(_localBasePath.Length - boundaryIndex)
                            .Skip(expIndex)
                        )
                    );
        });
    }

    private void DoActionOnExp(Action<int, int> func, int offset = 0)
    {
        int expIndexStart = -1, expIndexEnd;
        for (int i = 0; i < _localBasePath.Length; ++i)
        {
            var c = _localBasePath[i];

            if (c == '[') { expIndexStart = i + offset; continue; }
            if (c == ']' & expIndexStart != -1)
            {
                expIndexEnd = i - offset;
                func(expIndexStart, expIndexEnd);
                continue;
            }
        }
    }

    private void Parse(string str)
    {
        Queue<int> q = [];
        foreach (var numberStr in str.Split("-"))
        {
            try
            {
                q.Enqueue(Int32.Parse(numberStr.Split('\\')[0]));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to parse string {numberStr}\n{ex.StackTrace}");
            }

        };

        int defVal = q.Dequeue();

        _expressions.Add(
            new Expression(
                defVal,
                defVal,
                q.Dequeue(),
                Int32.Parse(str.Split('\\')[1])
                )
            );

        lock (this)
        {
            DoActionOnExp((start, end) => _localBasePath = _localBasePath.Replace(_localBasePath[start..end], $"[]"), -1);
        }
        /*_expressions.Count*/
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="BasePath"></param>
    /// <returns></returns>
    public string GetParsedPath()
    {
        int expNo = 0;
        string result = new(_localBasePath);

        DoActionOnExp((start, end) =>
        {
            var exp = _expressions.ElementAt(expNo);
            result = result.Replace(_localBasePath[start..end], exp.CurrentVal.ToString());

            if (exp.MaxVal == exp.CurrentVal) { exp.CurrentVal = exp.MinVal; }
            else { exp.CurrentVal += exp.ActionVal; }
        }, -1);

        return result;
    }
}
