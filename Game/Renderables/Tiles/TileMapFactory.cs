using Game.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Renderables.Tiles;

#nullable enable

public class TileMapFactory
    : IFactory<TileMap>
{
    Texture2D _texture;
    Tracker<Rectangle> _tilePosTracker;
    public UpdateAction<Rectangle>? UpdateActionOn2dChange { get; init; }
    /// <summary>
    /// Will be used to call <see cref="Tracker{T}.OffsetBy(UpdateAction{T}, object[]?)"/> on <see cref="TileMapFactory._tilePosTracker"/> with its <see cref="Tracker{T}.CurrentVal"/> value
    /// </summary>
    //Tracker<Rectangle>? _tilePosTracker2;

    /// <summary>
    /// Please, make sure that startPos is offset by -1 <paramref name="updateAction"/> call;<br/>
    /// <paramref name="updateAction"/> calls before returning value, not after 
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="updateAction"></param>
    public TileMapFactory(Texture2D texture,
                          Point startPos,
                          int endPosX,
                          UpdateAction<Rectangle> updateAction)
    {
        var temp = new Rectangle(startPos, texture.Bounds.Size);
        _texture = texture;
        _tilePosTracker = new(
            currentVal: temp,
            maxVal: new Rectangle(new(endPosX, startPos.Y), temp.Size),
            actionUpdate: updateAction,
            comparison: (r1, r2) => (r1 == r2) ? 0 : 1
            );
    }

    /// <summary>
    /// Default (1d) implementation of <see cref="IFactory{TileMap}"/>
    /// </summary>
    /// <returns>Complete <see cref="TileMap"/> object</returns>
    public TileMap Produce()
    {
        TileMap tm = new();

        Rectangle currentPos;
        while (_tilePosTracker.GetVal(out currentPos))
        {
            tm.SetTile(new Tile(currentPos, _texture));
        }

        return tm;
    }

    /// <summary>
    /// 2d implementation of <see cref="IFactory{TileMap}"/> <br/>
    /// Requires <see cref="TileMapFactory.UpdateActionOn2dChange"/> to be created in initializator
    /// </summary>
    /// <returns>Complete <see cref="TileMap"/> object</returns>
    /// <param name="iterationCount">The amount of times <see cref="TileMapFactory._tilePosTracker"/> resets its iterator
    /// before the <see cref="TileMap"/> object is fully made <br/>
    /// Or the "Height" of the <see cref="TileMap"/></param>
    public TileMap Produce2(int iterationCount)
    {
        if (UpdateActionOn2dChange is null)
            throw new System.ArgumentNullException(nameof(UpdateActionOn2dChange));
        if (iterationCount <= 0) throw new System.IndexOutOfRangeException(nameof(iterationCount));

        TileMap tm = new();

        // First call always results in "false";
        // assuming CurrentVal was offset by -1
        Rectangle currentPos = _tilePosTracker.CurrentVal;

        for (int i = 0; i < iterationCount; ++i)
        {
            do
            {
                tm.SetTile(new Tile(currentPos, _texture));
            } while (_tilePosTracker.GetVal(out currentPos));

            _tilePosTracker.OffsetBy(UpdateActionOn2dChange, null);
            currentPos = _tilePosTracker.GetValNoUpdate();
        }

        return tm;
    }

}