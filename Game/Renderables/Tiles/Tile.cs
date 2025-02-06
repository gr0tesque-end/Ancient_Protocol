using System.Collections.Generic;
using System.Diagnostics;
using Game.Misc;
using Game.Renderables.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Renderables.Tiles;

public class Tile
    : IRenderable
{
    public static readonly Point textureSizeP = new Point(64, 64);
    public static readonly Vector2 textureSizeV = new Vector2(64, 64);
    public Texture2D Texture { get; private set; }
    public Rectangle Bounds { get; }

    public Tile(Rectangle bounds, Texture2D texture)
    {
        Texture = texture;
        Bounds = bounds;
    }

    public static Dictionary<Rectangle, Wall> EncaseTiles(
        Dictionary<Rectangle, IRenderable> globalTileMap,
        Texture2D wall,
        Dictionary<string, Texture2D> Textures)
    {
        // Offsets for the four cardinal directions
        var directions = new Dictionary<Direction, Point>
    {
        { Direction.N, new Point(0, -textureSizeP.X) },
        { Direction.S, new Point(0, textureSizeP.X) },
        { Direction.W, new Point(-textureSizeP.X, 0) },
        { Direction.E, new Point(textureSizeP.X, 0) }
    };

        var wallsToAdd = new List<KeyValuePair<Rectangle, Wall>>();

        foreach (var (rect, tile) in globalTileMap)
        {
            foreach (var (side, offset) in directions)
            {
                Rectangle neighbor = new(
                    rect.X + offset.X,
                    rect.Y + offset.Y,
                    textureSizeP.X, textureSizeP.X);


                if (!globalTileMap.ContainsKey(neighbor))
                {
                    Wall wallTile = new Wall(neighbor, wall, side);
                    wallsToAdd.Add(new(neighbor, wallTile));
                }
            }
        }

        return Wall.MergeAllWallsIfPossible(wallsToAdd, Textures);
    }
    public virtual void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Bounds, Color.White);
    }

    public void LoadContent(ContentManager content)
    {}

    public void Update(GameTime gameTime)
    {}
}
