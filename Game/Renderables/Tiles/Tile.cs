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
    public Vector2 Position { get; set; }
    
    public Texture2D Texture { get; private set; }

    public Rectangle Bounds { get; private set; }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, Color.White);
    }

    public void LoadContent(ContentManager content)
    {
        Texture = content.Load<Texture2D>("Tiles/Grass/Grass1");
        Bounds = new((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
        
    }

    public void Update(GameTime gameTime)
    {}
}
