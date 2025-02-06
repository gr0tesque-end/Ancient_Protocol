using Game.Misc;
using Game.Renderables.Tiles.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Renderables.Tiles;

#nullable enable

public class TileMapFactory<T> : IFactory<TileMap>
    where T: Tile
{
    Texture2D _texture;
    private readonly IShapeProvider _shapeProvider;

    /// <summary>
    /// </summary>
    /// <param name="texture">Texture used throughout the TileMap</param>
    /// <param name="shapeProvider"></param>
    /// <param name="tiles"></param>
    /// <param name="startPos"></param>
    public TileMapFactory(Texture2D texture, IShapeProvider shapeProvider)
    {
        _texture = texture;
        _shapeProvider = shapeProvider;
    }
    public TileMap Produce()
    {
        TileMap tileMap = new();

        foreach (var pos in _shapeProvider.GetTilePositions())
        {
            tileMap.SetTile(new Tile(new Rectangle(pos, _texture.Bounds.Size), _texture));
        }

        return tileMap;
    }

}