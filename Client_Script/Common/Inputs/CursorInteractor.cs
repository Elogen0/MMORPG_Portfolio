using UnityEngine;
using UnityEngine.Events;
using Kame.Define;

public class CursorInteractor : MonoBehaviour
{
    [SerializeField] CursorType _cursorType = CursorType.None;
    public bool active = true;
    public UnityEvent OnClick;

    public CursorType CurrentCursorType
    {
        get { return _cursorType; }
        set { _cursorType = value; }
    }
}


