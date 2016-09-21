using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace QFramework.UI {

	/// <summary>
	/// 事件监听器
	/// </summary>
	public class UGUIEventListener : UnityEngine.EventSystems.EventTrigger
	{
		public QVoidDelegate.WithGo onClick;
		public QVoidDelegate.WithGo onDown;
		public QVoidDelegate.WithGo onEnter;
		public QVoidDelegate.WithGo onExit;
		public QVoidDelegate.WithGo onUp;
		public QVoidDelegate.WithGo onSelect;
		public QVoidDelegate.WithGo onUpdateSelect;

		public QVoidDelegate.WithEvent onBeginDrag;
		public QVoidDelegate.WithEvent onEndDrag;
		public QVoidDelegate.WithEvent onDrag;

		public QVoidDelegate.WithBool onValueChanged;

		public static UGUIEventListener CheckAndAddListener(GameObject go)
		{
			UGUIEventListener listener = go.GetComponent<UGUIEventListener>();
			if (listener == null) listener = go.AddComponent<UGUIEventListener>();

			return listener;
		}
		public static  UGUIEventListener Get(GameObject go)
		{
			return CheckAndAddListener (go);
		}

		public override void OnPointerClick(PointerEventData eventData)
		{
			if (onClick != null) onClick(gameObject);
		}
		public override void OnPointerDown(PointerEventData eventData)
		{
			if (onDown != null) onDown(gameObject);
		}
		public override void OnPointerEnter(PointerEventData eventData)
		{
			if (onEnter != null) onEnter(gameObject);
		}
		public override void OnPointerExit(PointerEventData eventData)
		{
			if (onExit != null) onExit(gameObject);
		}
		public override void OnPointerUp(PointerEventData eventData)
		{
			if (onUp != null) onUp(gameObject);
		}
		public override void OnSelect(BaseEventData eventData)
		{
			if (onSelect != null) onSelect(gameObject);
		}
		public override void OnUpdateSelected(BaseEventData eventData)
		{
			if (onUpdateSelect != null) onUpdateSelect(gameObject);
		}
		public override void OnBeginDrag(PointerEventData eventData)
		{
			if (onBeginDrag != null) onBeginDrag(eventData);
		}
		public override void OnEndDrag(PointerEventData eventData)
		{
			if (onEndDrag != null) onEndDrag(eventData);
		}
		public override void OnDrag(PointerEventData eventData) 
		{
			if (onDrag != null) onDrag(eventData);
		}
	}
}