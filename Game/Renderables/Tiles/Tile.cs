using System.Diagnostics;
using Game.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Renderables.Tiles;

public class Tile
    : IRenderable
{
    public static readonly Vector2 textureSize = new Vector2(64, 64);
    public Vector2 Position => throw new System.NotImplementedException();
    public Texture2D Texture { get; private set; }
    public Rectangle Bounds { get; }

    public Tile(Vector2 position, Texture2D texture)
    {
        Texture = texture;
        Bounds = new((int)position.X, (int)position.Y, Texture.Width, Texture.Height);
    }

    public Tile(Rectangle bounds, Texture2D texture)
    {
        Texture = texture;
        Bounds = bounds;
    }


    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Bounds, Color.White);
    }

    public void LoadContent(ContentManager content)
    {}

    public void Update(GameTime gameTime)
    {}
}
