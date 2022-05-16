using System;
using System.Collections.Generic;
using DG.Tweening;
using Kame;
using Kame.Define;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Diagnostics;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class UIManager : SingletonMono<UIManager>
{
    private TransformAnchor _anchor;
    #region MonoBehaviour
    protected override void Awake()
    {
        base.Awake();
        MakeWorldSpaceUI();
        MakeOverlayCanvas();
        MakeEventSystem();
    }

    private void MakeEventSystem()
    {
        EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
        if (eventSystem == null)
            AddressableLoader.InstantiateAsync("Assets/Game/Prefab/UI/Components/EventSystem.prefab", transform,DontDestroyOnLoad);
        else
        {
            
            DontDestroyOnLoad(eventSystem);
            //eventSystem.transform.SetParent(transform);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (worldSpaceCanvas)
            worldSpaceCanvas.worldCamera = Camera.main;
    }
    #endregion
    
    #region CanvasSetting
    private Canvas worldSpaceCanvas;
    public Canvas WorldSpaceCanvas
    {
        get
        {
            if (worldSpaceCanvas == null)
                MakeWorldSpaceUI();
            return worldSpaceCanvas;
        }
    }

    private void MakeWorldSpaceUI()
    {
        GameObject canvasGO = new GameObject();
        canvasGO.name = "WorldSpaceCanvas";
        DontDestroyOnLoad(canvasGO);
        canvasGO.AddComponent<Canvas>();
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();
        canvasGO.GetComponent<RectTransform>().localScale = new Vector3(0.01f, 0.01f, 0.01f);
        canvasGO.layer = LayerMask.NameToLayer("UI");

        worldSpaceCanvas = canvasGO.GetComponent<Canvas>();
        worldSpaceCanvas.renderMode = RenderMode.WorldSpace;
        worldSpaceCanvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;
        worldSpaceCanvas.worldCamera = Camera.main;
    }
    
    public void ClearAllWorldSpaceUI()
    {
        foreach (Transform tr in worldSpaceCanvas.transform)
        {
            ResourceLoader.Destroy(tr.gameObject);
        }
    }
    
    private Canvas overlayCanvas;

    public Canvas OverlayCanvas
    {
        get
        {
            if (overlayCanvas == null)
                MakeOverlayCanvas();
            return overlayCanvas;
        }
    }
    
    [NonSerialized] public UI_PopupBlocker PopupBlocker;
    
    protected void MakeOverlayCanvas()
    {
        // Canvas
        GameObject CanvasGO = new GameObject();
        CanvasGO.name = "PopupCanvas";
        DontDestroyOnLoad(CanvasGO);
        overlayCanvas = CanvasGO.AddComponent<Canvas>();
        overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        overlayCanvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;
        overlayCanvas.sortingOrder = 100;
        
        CanvasGO.AddComponent<CanvasScaler>();
        CanvasGO.AddComponent<GraphicRaycaster>();
        AddressableLoader.InstantiateAsync("Assets/Game/Prefab/UI/Components/PopupBlocker.prefab", null,go =>
        {
            PopupBlocker = go.GetComponent<UI_PopupBlocker>();
            PopupBlocker.gameObject.SetActive(false);    
        });
    }
    
    public void ClearAllOverlayCanvasUI()
    {
        foreach (Transform tr in overlayCanvas.transform)
        {
            ResourceLoader.Destroy(tr.gameObject);
        }
    }
    
    private int _order = 10;
    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = go.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if (sort)
        {
            canvas.sortingOrder = _order++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }
    #endregion


    #region Methods
    private UI_Scene _sceneUI;
    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;
        
        if (_sceneUI)
        {
            ResourceLoader.Destroy(_sceneUI.gameObject);
            _sceneUI = null;
        }

        GameObject go = ResourceLoader.Instantiate(AssetPath.UI_Scene+name);
        T sceneUI = go.GetOrAddComponent<T>();
        _sceneUI = sceneUI;
        sceneUI.transform.SetParent(overlayCanvas.transform);
        return sceneUI;
    }
    
    public T ShowUI<T>(string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;
        GameObject go = ResourceLoader.Instantiate(AssetPath.UI+name);
        SetCanvas(go);
        return go.GetComponent<T>();
    }

    public void CloseUI<T>(GameObject go) where T :UI_Base
    {
        if (!go)
            return;
        
        ResourceLoader.Destroy(go);
        --_order;
    }
    #endregion

    #region Popup
    private static Queue<UI_Popup> waitingPopups = new Queue<UI_Popup>();
    private static Stack<UI_Popup> showingPopups = new Stack<UI_Popup>();

    public T ShowPopup<T>(string name = null, bool wait = true) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;
        
        SetCanvas(PopupBlocker.gameObject);
        PopupBlocker.gameObject.SetActive(true);
        
        GameObject go = ResourceLoader.Instantiate($"{AssetPath.UI_Popup}{name}.prefab", Instance.overlayCanvas.transform);
        if (go == null)
            return null;
        SetCanvas(go);
        T popup = go.GetComponent<T>();
        if (showingPopups.Count > 0 && wait)
        {
            waitingPopups.Enqueue(popup);
            go.gameObject.SetActive(false);
            return popup;
        }
        
        showingPopups.Push(popup);
        transform.DOScale(new Vector2(1f, 1f), 0.1f).From(new Vector2(.2f, .2f)).SetEase((Ease.OutElastic));
        
        return popup;
    }

    private void ShowNextPopup()
    {
        if (showingPopups.Count > 0)
            return;
        if (waitingPopups.Count <= 0)
            return;

        UI_Popup popup = waitingPopups.Dequeue();
        showingPopups.Push(popup);
        popup.gameObject.SetActive(true);
        popup.transform.DOScale(new Vector2(1f, 1f), 0.1f).From(new Vector2(.2f, .2f)).SetEase((Ease.OutElastic));
    }
    
    public void ClosePopup()
    {
        if (showingPopups.Count <= 0)
            return;

        UI_Popup popup = showingPopups.Pop();
        popup.transform.DOScale(new Vector3(0, 0), .1f).SetEase(Ease.Linear);
        popup.GetComponent<CanvasGroup>().DOFade(0, .1f).OnComplete(()=>
        {
            ResourceLoader.Destroy(popup.gameObject);
            ShowNextPopup();
        });

        if (showingPopups.Count <= 0 && waitingPopups.Count <= 0)
        {
            PopupBlocker.gameObject.SetActive(false);
        }
    }

    public void ClearAllPopup()
    {
        foreach (var popup in showingPopups)
        {
            ResourceLoader.Destroy(popup.gameObject);
            _order--;
        }
        showingPopups.Clear();

        foreach (var popup in waitingPopups)
        {
            ResourceLoader.Destroy(popup.gameObject);
            _order--;
        }
        waitingPopups.Clear();
        
        PopupBlocker.gameObject.SetActive(false);
    }
    

    #endregion
    
}
