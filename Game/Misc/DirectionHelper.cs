using Game.Renderables.Player;
using System.Collections.Generic;

namespace Game.Misc;
public static class DirectionHelper
{
    public static readonly Direction[] ClockwiseOrder =
    {
        Direction.SE,
        Direction.S,
        Direction.SW,
        Direction.W,
        Direction.NW,
        Direction.N,
        Direction.NE,
        Direction.E
    };

    public static int GetClockwiseIndex(Direction dir)
    {
        for (int i = 0; i < ClockwiseOrder.Length; i++)
        {
            if (ClockwiseOrder[i] == dir) return i;
        }
        return -1;
    }
}