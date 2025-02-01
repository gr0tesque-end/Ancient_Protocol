using System.Collections.Generic;

namespace Game.Misc;

public interface IFactory<T>
{
    public T Produce();
}
