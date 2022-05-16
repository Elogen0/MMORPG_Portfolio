using System;
using System.Collections;
using System.Collections.Generic;

namespace InflearnServer.Game.Data
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
    }

    public interface IManagedData
    {
        int ID { get; }
    }
}

