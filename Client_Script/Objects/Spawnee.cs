using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Spawnee
{
    public IEnumerator AfterSpawnAction(GameObject caller, object data);
}
