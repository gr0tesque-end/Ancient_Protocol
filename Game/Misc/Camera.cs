using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Game.Misc;

public class Camera
{
    public Matrix Transform { get; private set; }

    public void Follow(ICamareFollowable target, Viewport viewport, Vector2 textureSize)
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
    }
}