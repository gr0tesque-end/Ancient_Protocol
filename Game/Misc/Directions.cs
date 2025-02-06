using System;

namespace Game.Misc;

[Flags]
public enum Direction : byte
{
    N = 2,    // North
    S = 4,    // South
    W = 8,    // West
    E = 16,   // East

    SE = S | E, // South-East
    SW = S | W, // South-West
    NW = N | W, // North-West
    NE = N | E  // North-East

}