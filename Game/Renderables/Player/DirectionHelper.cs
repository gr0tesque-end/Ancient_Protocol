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

    public static readonly Dictionary<byte, Direction> byteToDirWalls = new()
    {
        { 6,  Direction.N },
        { 20, Direction.E },//
        { 18, Direction.N },///
        { 22, Direction.E },////
        { 12, Direction.S },///
        { 10, Direction.W },//
        { 14, Direction.W },////
        { 24, Direction.W },
        { 28, Direction.S },/////
        { 26, Direction.N },/////
        { 30, Direction.W }
    };

    public static bool AreOpposite(Direction dir1, Direction dir2)
    {
        return (dir1 & dir2) == 0 && (dir1 | dir2) == (Direction.N | Direction.S)
                                  || (dir1 | dir2) == (Direction.W | Direction.E);
    }
}