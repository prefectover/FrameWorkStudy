using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace QFramework.UI {

	/// <summary>
	/// 事件监听器
	/// </summary>
	public class UGUIEventListener : UnityEngine.EventSystems.EventTrigger
	{
		public QVoidDelegate.WithVoid onClick;
	
		public QVoidDelegate.WithGo onSelect;
		public QVoidDelegate.WithGo onUpdateSelect;

		public QVoidDelegate.WithEvent onPointerDown;
		public QVoidDelegate.WithEvent onPointerEnter;
		public QVoidDelegate.WithEvent onPointerExit;
		public QVoidDelegate.WithEvent onPointerUp;

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
			if (onClick != null) onClick();
		}
		public override void OnPointerDown(PointerEventData eventData)
		{
			if (onPointerDown != null) onPointerDown(eventData);
		}
		public override void OnPointerEnter(PointerEventData eventData)
		{
			if (onPointerEnter != null) onPointerEnter(eventData);
		}
		public override void OnPointerExit(PointerEventData eventData)
		{
			if (onPointerExit != null) onPointerExit(eventData);
		}
		public override void OnPointerUp(PointerEventData eventData)
		{
			if (onPointerUp != null) onPointerUp(eventData);
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