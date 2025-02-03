using Game.Renderables.Tiles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Game.Renderables;

public interface IRenderableContainer
{
    public Dictionary<Rectangle, IRenderable> Container { get; }


}