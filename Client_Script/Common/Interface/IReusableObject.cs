using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kame
{
    public interface IReusableObject
    {
        void AwakeObject();
        void SleepObject();
    }
}
