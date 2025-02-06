using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

    public TileMap(Dictionary<Rectangle, IRenderable> dict)
    {
        Container = dict;
    }

    public TileMap() { }

    public void SetTile(Tile tile)
    {
        Container[tile.Bounds] = tile;
    }

    public IRenderable GetTile(int x, int y)
    {
        return Container.TryGetValue(
            new Rectangle(new Point(x, y), Tile.textureSizeP),
            out var tile) ? tile : null;
    }

    public TileMap AddToGlobalTileMap(ref Dictionary<Rectangle, IRenderable> gtm)
    {
        try
        {
            gtm = gtm.Concat(Container).ToDictionary();
        }
        catch (System.ArgumentException e)
        {
            Debug.WriteLine(e.Message);
        }
        return this;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var (pos, tile) in Container)
        {
            if (!Camera.ViewRectangle.Contains(pos.Location)) continue;
            /*spriteBatch.Draw(tile.Value.Texture, tile.Key, Color.White);*/
            tile.Draw(spriteBatch);
        }
    }

    public void Update(GameTime gameTime) { }

    public void LoadContent(ContentManager content) { }
}
