using System.Collections;
using System.Collections.Generic;

namespace InflearnServer.Game.Data
{
    public enum ModifierType
    {
        Flat,
        PercentAdd,
        PercentMult,
    }
    
    public interface IModifier<T>
    {
        void AddValue(ref T baseValue);
        void PercentAddValue(ref T baseValue);
        void PercentMultValue(ref T baseValue);
    }
}

