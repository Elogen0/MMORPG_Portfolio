using System;
using System.Collections;
using System.Collections.Generic;
using Kame.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum UIEvent
{
	Click,
	Drag,
}

public abstract class UI_Base : MonoBehaviour
{
	protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();
	public abstract void Init();

	protected virtual void Awake()
	{
		Init();
	}

	protected void Bind<T>(Type type, bool recursive = true) where T : UnityEngine.Object
	{
		string[] names = Enum.GetNames(type);
		UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
		_objects.Add(typeof(T), objects);

		for (int i = 0; i < names.Length; i++)
		{
			if (typeof(T) == typeof(GameObject))
				objects[i] = GameUtil.FindChild(gameObject, names[i], recursive);
			else
				objects[i] = GameUtil.FindChild<T>(gameObject, names[i], recursive);

			if (objects[i] == null)
				Debug.Log($"Failed to bind({names[i]})");
		}
	}

	public T Get<T>(int idx) where T : UnityEngine.Object
	{
		UnityEngine.Object[] objects = null;
		if (_objects.TryGetValue(typeof(T), out objects) == false)
			return null;

		return objects[idx] as T;
	}

	public GameObject GetObject(int idx) { return Get<GameObject>(idx); }
	public TextMeshProUGUI GetText(int idx) { return Get<TextMeshProUGUI>(idx); }
	public Button GetButton(int idx) { return Get<Button>(idx); }
	public Image GetImage(int idx) { return Get<Image>(idx); }
	
	public void SetText(int idx, string message)
	{
		TextMeshProUGUI text = GetText(idx);
		if (text)
			text.text = message;
	}

	public void SetImage(int idx, Sprite sprite)
	{
		Image img = GetImage(idx);
		if (img)
			img.sprite = sprite;
	}

	public static void BindEvent(GameObject go, Action<PointerEventData> action, UIEvent type = UIEvent.Click)
	{
		UI_EventHandler evt = GameUtil.GetOrAddComponent<UI_EventHandler>(go);

		switch (type)
		{
			case UIEvent.Click:
				evt.OnClickHandler -= action;
				evt.OnClickHandler += action;
				break;
			case UIEvent.Drag:
				evt.OnDragHandler -= action;
				evt.OnDragHandler += action;
				break;
		}
	}
}
