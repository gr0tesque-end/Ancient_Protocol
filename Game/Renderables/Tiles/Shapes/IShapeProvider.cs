using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Game.Renderables.Tiles.Shapes;

public interface IShapeProvider
{
    IEnumerable<Point> GetTilePositions();
}