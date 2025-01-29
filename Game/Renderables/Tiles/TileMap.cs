using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Renderables.Tiles;
public class TileMap
    : IRenderable
{
    public Vector2 Position => throw new NotImplementedException();
    public Rectangle Bounds { get; private set; }
    public void Draw(SpriteBatch spriteBatch)
    {
        throw new NotImplementedException();
    }

    public void LoadContent(ContentManager content)
    {
        throw new NotImplementedException();
    }

    public void Update(GameTime gameTime)
    {
        throw new NotImplementedException();
    }
}
