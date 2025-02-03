using System;
using System.Collections.Generic;
using System.Threading;
using Game.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Renderables.Tiles;
public class TileMap
    : IRenderableContainer, IRenderable
{
    public Dictionary<Rectangle, IRenderable> Container { get; private set; } = new();

    public Rectangle Bounds => throw new NotImplementedException();

    public Texture2D Texture { get; private set; }

    public void SetTile(Tile tile)
    {
        Container[tile.Bounds] = tile;
    }

    public IRenderable GetTile(int x, int y)
    {
        return Container.TryGetValue(
            new Rectangle(new Point(x, y), Tile.textureSize.ToPoint()),
            out var tile) ? tile : null;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var tile in Container)
        {
            if (!Camera.ViewRectangle.Contains(tile.Key))
            {
                continue;
            }
            spriteBatch.Draw(tile.Value.Texture, tile.Key, Color.White);
        }
    }

    public void Update(GameTime gameTime) { }

    public void LoadContent(ContentManager content) { }
}
