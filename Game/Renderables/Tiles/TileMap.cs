using System;
using System.Collections.Generic;
using System.Threading;
using Game.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Renderables.Tiles;
public class TileMap
    : IRenderable
{
    public Vector2 Position { get; private set; }
    public Rectangle Bounds { get; private set; }

    public Dictionary<Rectangle, Tile> tiles { get; init; } = new Dictionary<Rectangle, Tile>();
    public void SetTile(Tile tile)
    {
        tiles[tile.Bounds] = tile;
    }

    public Tile GetTile(int x, int y)
    {
        return tiles.TryGetValue(
            new Rectangle(new Point(x, y), Tile.textureSize.ToPoint()),
            out Tile tile) ? tile : null;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var tile in tiles)
        {
            if (!Camera.ViewRectangle.Contains(tile.Key))
            {
                continue;
            }
            spriteBatch.Draw(tile.Value.Texture, tile.Key, Color.White);
        }
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
