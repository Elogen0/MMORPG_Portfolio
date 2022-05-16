using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class SpriteFitToScreen : MonoBehaviour
{
    public void Resize()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        transform.localScale = new Vector3(1, 1, 1);

        float width = sr.sprite.bounds.size.x;
        float height = sr.sprite.bounds.size.y;

        float worldScreenHeight = Camera.main.orthographicSize * 2f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        Vector3 xWidth = transform.localScale;
        xWidth.x = worldScreenHeight / width;
        transform.localScale = xWidth;

        Vector3 yHeight = transform.localScale;
        yHeight.y = worldScreenWidth / height;
        transform.localScale = yHeight;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SpriteFitToScreen))]
    public class SpriteFitToScreenEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Fit to Screen"))
            {
                SpriteFitToScreen sf = target as SpriteFitToScreen;
                sf.Resize();    
            }
        }
    }
#endif
}
