using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Game.Renderables.Tiles.Shapes;

public class RectangleShapeProvider : IShapeProvider
{
    private readonly Point _startPos;
    public bool isFilled;
    public int diameterX;
    public int diameterY;

    public RectangleShapeProvider(Point startPos, int radiusX, int radiusY, bool fill)
    {
        _startPos = startPos;
        diameterX = radiusX * 2 + 1; // Allows odd-numbered widths
        diameterY = radiusY * 2 + 1; // Allows odd-numbered heights
        isFilled = fill;
    }

    public IEnumerable<Point> GetTilePositions()
    {
        if (!isFilled)
        {
            foreach (var point in GetTilePositionsNoFill())
                yield return point;
            yield break;
        }

        for (int y = 0; y < diameterY; y++)
        {
            for (int x = 0; x < diameterX; x++)
            {
                yield return new Point(_startPos.X + x * 64, _startPos.Y + y * 64);
            }
        }
    }

    private IEnumerable<Point> GetTilePositionsNoFill()
    {
        // Top border
        for (int x = 0; x < diameterX; x++)
        {
            yield return new Point(_startPos.X + x * 64, _startPos.Y);
        }

        // Middle section: only left and right borders
        for (int y = 1; y < diameterY - 1; y++)
        {
            yield return new Point(_startPos.X, _startPos.Y + y * 64);                  // Left border
            yield return new Point(_startPos.X + (diameterX - 1) * 64, _startPos.Y + y * 64); // Right border
        }

        // Bottom border
        for (int x = 0; x < diameterX; x++)
        {
            yield return new Point(_startPos.X + x * 64, _startPos.Y + (diameterY - 1) * 64);
        }
    }
}
