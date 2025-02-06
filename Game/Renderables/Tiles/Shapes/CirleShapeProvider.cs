using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Renderables.Tiles.Shapes;

public class CircleShapeProvider
    : IShapeProvider
{
    private Rectangle _startPos;
    public int radiusX;
    public int radiusY;
    private Texture2D _texture;
    public bool isFilled;

    /// <summary>
    /// Value betweer 1.0f - 1.5f
    /// <br/>My preferrence: 1.2f
    /// </summary>
    public float Precision { get; init; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="texture">Used for dimensions purposes only</param>
    /// <param name="startPos"></param>
    /// <param name="radiusX"></param>
    /// <param name="radiusY"></param>
    /// <param name="fill"></param>
    public CircleShapeProvider(Texture2D texture,
                               Rectangle startPos,
                               int radiusX,
                               int radiusY,
                               bool fill)
    {
        _startPos = startPos;
        this.radiusX = radiusX;
        this.radiusY = radiusY;
        _texture = texture;
        isFilled = fill;
    }

    public IEnumerable<Point> GetTilePositions()
    {
        List<Point> positions = new();

        for (int y = -radiusY; y <= radiusY; y++)
        {
            for (int x = -radiusX; x <= radiusX; x++)
            {
                int tileX = _startPos.X + x * _texture.Width;
                int tileY = _startPos.Y + y * _texture.Height;

                double ellipseValue = (x * x) / (double)(radiusX * radiusX) + 
                                      (y * y) / (double)(radiusY * radiusY);

                if (isFilled && ellipseValue <= Precision)
                    positions.Add(new Point(tileX, tileY));
                else
                {
                    double borderThreshold = 0.15;
                    if (ellipseValue >= 1 - borderThreshold && ellipseValue <= Precision)
                        positions.Add(new Point(tileX, tileY));
                }
            }
        }
        return positions;
    }
}
