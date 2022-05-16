using System.Collections;
using System.Collections.Generic;

namespace Kame.Game.Data
{
    public enum ModifierType
    {
        Flat,
        PercentAdd,
        PercentMult,
    }
    
    public interface IModifier<T>
    {
        void AddValue(ref T value);
        void PercentAddValue(ref T value);
        void PercentMultValue(ref T value);
    }
}

