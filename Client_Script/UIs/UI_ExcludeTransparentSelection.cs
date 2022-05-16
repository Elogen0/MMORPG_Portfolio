using UnityEngine;
using UnityEngine.UI;

public class UI_ExcludeTransparentSelection : MonoBehaviour
{
    void Start()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
    }
}
