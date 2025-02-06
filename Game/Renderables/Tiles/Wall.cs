using System.Collections.Generic;
using System.Linq;
using Game.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Renderables.Tiles;

public class Wall : 
    Tile, IPositionTrackable
{
    float _rotation = 0;
    public Direction neighbouringWallDirection;

    public Vector2 Position { get; private set; }

    public Wall(Rectangle bounds, Texture2D texture) : base(bounds, texture) {
        Position = new Vector2(Bounds.X + Bounds.Width / 2, Bounds.Y + Bounds.Height / 2);
    }

    public Wall(Rectangle bounds, Texture2D texture, Direction side)
        : this(bounds, texture)
    {
        neighbouringWallDirection = side;
        _rotation = side switch
        {
            Direction.N => 1.5708f,
            Direction.S => -1.5708f,
            Direction.W => 0,
            Direction.E => 3.14159f,
            _ => throw new System.InvalidOperationException($"Unsupported side for Wall: {side}")
        };
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        /*base.Draw(spriteBatch);
        return;*/
        spriteBatch.Draw(
            texture: Texture,
            position: Position,
            sourceRectangle: null,
            color: Color.White,
            rotation: _rotation,
            origin: new Vector2(Texture.Width / 2, Texture.Height / 2),
            scale: 1.1f,
            effects: SpriteEffects.None,
            layerDepth: 0
    );
    }
    public static Wall MergeWalls(Wall[] walls, Dictionary<string, Texture2D> Textures)
    {
        int wallsLen = walls.Length;
        Direction[] neighbouringWallDirections = walls.Select(w => w.neighbouringWallDirection).ToArray();

        int textureNum = wallsLen switch
        {
            2 => DirectionHelper.AreOpposite(neighbouringWallDirections[0],
                                             neighbouringWallDirections[1])
                    ? 5
                    : 2,
            _ => wallsLen
        };
        var texture = Textures[
            $"{new string(walls[0].Texture.Name
                    .SkipLast(1)
                    .ToArray())
                .Split('/').Last()}{textureNum}"];

        return new(
            walls[0].Bounds,
            texture,
            DirectionHelper.byteToDirWalls[
                (byte)neighbouringWallDirections.Aggregate((a, b) => a | b)
                ]
            );
    }

    public static Dictionary<Rectangle, Wall> MergeAllWallsIfPossible(
         List<KeyValuePair<Rectangle, Wall>> unmergedWalls,
         Dictionary<string, Texture2D> Textures)
    {
        var grouped = unmergedWalls.GroupBy(kv => kv.Key);

        Dictionary<Rectangle, Wall> result = [];


        foreach (var group in grouped)
        {
            var values = group.Select(kv => kv.Value);

            if (group.Count() == 1)
            {
                result[group.Key] = values.ElementAt(0);
                continue;
            }

            result[group.Key] = MergeWalls(values.ToArray(), Textures);
        }
        return result;
    }
}
