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

    public static readonly Dictionary<Direction, int> dirIndex = new()
    {
        { Direction.SE, 0 },
        { Direction.S, 1 },
        { Direction.SW, 2 },
        { Direction.W, 3 },
        { Direction.NW, 4 },
        { Direction.N, 5 },
        { Direction.NE, 6 },
        { Direction.E, 7 }
    };
}