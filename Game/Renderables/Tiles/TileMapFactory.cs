using Game.Misc;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Renderables.Tiles;

public class TileMapFactory
    : IFactory<TileMap>
{
    Texture2D _texture;
    (Tracker, Tracker) _tilePosTracker;
    Microsoft.Xna.Framework.Vector2 _currentPos
    {
        get =>
            (isSquare) ? new(_tilePosTracker.Item1.CurrentVal, _tilePosTracker.Item1.CurrentVal) :
            new(_tilePosTracker.Item1.CurrentVal, _tilePosTracker.Item2.CurrentVal);
    }
    readonly bool isSquare = false;

    public TileMapFactory(Texture2D texture, Tracker posTracker)
    {
        _texture = texture;
        _tilePosTracker = (posTracker, posTracker);
        isSquare = true;
    }

    public TileMapFactory(Texture2D texture, (Tracker, Tracker) posTracker)
    {
        _texture = texture;
        _tilePosTracker = posTracker;
    }

    public TileMap Produce()
    {
        TileMap tm = new();

        return tm;
    }
}