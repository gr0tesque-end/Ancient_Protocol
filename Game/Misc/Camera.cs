using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Game.Renderables;
using Game.Renderables.Tiles;

namespace Game.Misc;

public class Camera
{
    public Matrix Transform { get; private set; }
    public Rectangle ViewRectangle { get; private set; }

    public void Follow(IRenderable target, Viewport viewport, Vector2 textureSize)
    {
        var position = Matrix.CreateTranslation(
            -(target.Position.X + textureSize.X / 2), // Offset X by half the texture width
            -(target.Position.Y + textureSize.Y / 2), // Offset Y by half the texture height
            0);

        var offset = Matrix.CreateTranslation(
            viewport.Width / 2,
            viewport.Height / 2,
            0);

        Transform = position * offset;

        ViewRectangle = GetViewRectangle(viewport);
    }
    private Rectangle GetViewRectangle(Viewport viewport)
    {
        Matrix inverseTransform = Matrix.Invert(Transform);

        Vector2 topLeft = Vector2.Transform(Vector2.Zero, inverseTransform);
        Vector2 bottomRight = Vector2.Transform(new Vector2(viewport.Width, viewport.Height), inverseTransform);

        return new Rectangle(
            (topLeft - Tile.textureSize).ToPoint(),
            (bottomRight - topLeft + Tile.textureSize).ToPoint());

    }
}