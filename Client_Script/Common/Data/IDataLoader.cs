using System;
using System.Collections;
using System.Collections.Generic;

namespace Kame.Game.Data
{
    public enum FileFormat
    {
        XML,
        Json,
        Web,
        ScriptableObject,
    }
    public interface IDataLoader<Key, Value>
    {
        Dictionary<Key, Value> MakeDic();
        int AddData(string newName);
        void RemoveData(int index);
        void Copy(int index);
    }

    public interface IManagedData
    {
        int ID { get; }
    }
}

