using System;

namespace Game.Misc;

#nullable enable

public delegate void UpdateAction(ref int val, object[]? args);
public delegate void UpdateAction<T>(ref T val, object[]? args);
public class Tracker
{
    private int _val;
    public int CurrentVal
    {
        get
        {
            OnUpdateVal.Invoke(ref _val, args);

            if (_val == MaxVal)
            {
                _val = MinVal;
            }
            return _val;
        }
    }
    public int MinVal;
    public int MaxVal;

    public UpdateAction OnUpdateVal;
    public object[]? args;

    public Tracker(int currentVal, int minVal, int maxVal, UpdateAction actionUpdate)
    {
        if (currentVal == maxVal) throw new Exception();
        _val = currentVal;
        MinVal = minVal;
        MaxVal = maxVal;
        OnUpdateVal = actionUpdate;
    }

    public Tracker(int maxVal, UpdateAction actionUpdate)
    {
        if (maxVal <= 0) throw new Exception();
        _val = -1;
        MinVal = 0;
        MaxVal = maxVal;
        OnUpdateVal = actionUpdate;
    }
    public Tracker(int maxVal)
    {
        if (maxVal <= 0) throw new Exception();
        _val = -1;
        MinVal = 0;
        MaxVal = maxVal;
        OnUpdateVal = (ref int val, object[]? args) => val += 1;
    }
};

/// <summary>
/// Customizable for loop behaviour<br/>
/// Define <see cref="Tracker{T}.args"/> for more complex logic
/// </summary>
/// <typeparam name="T"></typeparam>
public class Tracker<T>
{
    private T _val;
    public T CurrentVal
    {
        get
        {
            UpdateVal.Invoke(ref _val, args);

            if (ComparasonFunc.Invoke(_val, MaxVal) == 0)
            {
                _val = MinVal;
            }
            return _val;
        }
    }
    public T MinVal;
    public T MaxVal;

    public UpdateAction<T> UpdateVal;
    public Comparison<T> ComparasonFunc;
    public object[]? args;

    public void OffsetBy(UpdateAction<T> action, object[]? args)
    {
        action.Invoke(ref _val, args);
        action.Invoke(ref MinVal, args);
        action.Invoke(ref MaxVal, args);
    }

    /// <summary>
    /// Same as accessing <see cref="CurrentVal"/> 
    /// </summary>
    /// <param name="val"></param>
    /// <returns>"false" if the value has been reset to <see cref="Tracker{T}.MinVal"/> after retrieval</returns>
    public bool GetVal(out T val)
    {
        val = CurrentVal;
        return !(ComparasonFunc(MinVal, val) == 0);
    
    }

    public T GetValNoUpdate() { return _val; }

    public Tracker(T currentVal,
                   T minVal,
                   T maxVal,
                   UpdateAction<T> actionUpdate,
                   Comparison<T> comparison)
    {
        if (comparison(currentVal, maxVal) == 0) throw new Exception();
        _val = currentVal;
        MinVal = minVal;
        MaxVal = maxVal;
        UpdateVal = actionUpdate;
        ComparasonFunc = comparison;
    }

    /// <summary>
    /// <see cref="MinVal"/> is the same as
    /// <paramref name="currentVal"/>
    /// </summary>
    /// <param name="currentVal"></param>
    /// <param name="maxVal"></param>
    /// <param name="actionUpdate"></param>
    /// <param name="comparison"></param>
    public Tracker(T currentVal,
                   T maxVal,
                   UpdateAction<T> actionUpdate,
                   Comparison<T> comparison)
    {
        if (comparison(currentVal, maxVal) == 0) throw new Exception();
        _val = currentVal;
        actionUpdate(ref currentVal, null);
        MinVal = currentVal;
        MaxVal = maxVal;
        UpdateVal = actionUpdate;
        ComparasonFunc = comparison;
    }
};