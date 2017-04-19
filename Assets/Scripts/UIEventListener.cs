using UnityEngine;
using UnityEngine.EventSystems;

public class UIEventListener : EventTrigger {

    public static UIEventListener Get(GameObject go) {
        var el = go.GetComponent<UIEventListener>();
        if(el == null) {
            el = go.AddComponent<UIEventListener>();
        }
        return el;
    }

    public delegate void VoidDelegate(GameObject go);
    public delegate void BoolDelegate(GameObject go, bool isPress);
    public delegate void PointerEventDataDelegate(GameObject go, PointerEventData state);
    public delegate void BaseEventDataDelegate(GameObject go, BaseEventData state);
    public delegate void AxisEventDataDelegate(GameObject go, AxisEventData state);

    public VoidDelegate onClick;
    public BoolDelegate onPress;
    public PointerEventDataDelegate onBeginDrag;
    public BaseEventDataDelegate onCancel;
    public BaseEventDataDelegate onDeselect;
    public PointerEventDataDelegate onDrag;
    public PointerEventDataDelegate onDrop;
    public PointerEventDataDelegate onEndDrag;
    public PointerEventDataDelegate onInitializePotentialDrag;
    public AxisEventDataDelegate onMove;
    public PointerEventDataDelegate onPointerClick;
    public PointerEventDataDelegate onPointerDown;
    public PointerEventDataDelegate onPointerEnter;
    public PointerEventDataDelegate onPointerExit;
    public PointerEventDataDelegate onPointerUp;
    public PointerEventDataDelegate onScroll;
    public BaseEventDataDelegate onSelect;
    public BaseEventDataDelegate onSubmit;
    public BaseEventDataDelegate onUpdateSelected;

    public override void OnBeginDrag(PointerEventData eventData) {
        if(onBeginDrag != null)
            onBeginDrag(this.gameObject, eventData);
    }

    public override void OnCancel(BaseEventData eventData) {
        if(onCancel != null)
            onCancel(this.gameObject, eventData);
    }

    public override void OnDeselect(BaseEventData eventData) {
        if(onDeselect != null)
            onDeselect(this.gameObject, eventData);
    }

    public override void OnDrag(PointerEventData eventData) {
        if(onDrag != null)
            onDrag(this.gameObject, eventData);
    }

    public override void OnDrop(PointerEventData eventData) {
        if(onDrop != null)
            onDrop(this.gameObject, eventData);
    }

    public override void OnEndDrag(PointerEventData eventData) {
        if(onEndDrag != null)
            onEndDrag(this.gameObject, eventData);
    }

    public override void OnInitializePotentialDrag(PointerEventData eventData) {
        if(onInitializePotentialDrag != null)
            onInitializePotentialDrag(this.gameObject, eventData);
    }

    public override void OnMove(AxisEventData eventData) {
        if(onMove != null)
            onMove(this.gameObject, eventData);
    }

    public override void OnPointerClick(PointerEventData eventData) {
        if(onPointerClick != null)
            onPointerClick(this.gameObject, eventData);
        if(onClick != null)
            onClick(this.gameObject);
    }

    public override void OnPointerDown(PointerEventData eventData) {
        if(onPointerDown != null)
            onPointerDown(this.gameObject, eventData);
        if(onPress != null)
            onPress(this.gameObject, true);
    }

    public override void OnPointerEnter(PointerEventData eventData) {
        if(onPointerEnter != null)
            onPointerEnter(this.gameObject, eventData);
    }

    public override void OnPointerExit(PointerEventData eventData) {
        if(onPointerExit != null)
            onPointerExit(this.gameObject, eventData);
    }

    public override void OnPointerUp(PointerEventData eventData) {
        if(onPointerUp != null)
            onPointerUp(this.gameObject, eventData);
        if(onPress != null)
            onPress(this.gameObject, false);
    }

    public override void OnScroll(PointerEventData eventData) {
        if(onScroll != null)
            onScroll(this.gameObject, eventData);
    }

    public override void OnSelect(BaseEventData eventData) {
        if(onSelect != null)
            onSelect(this.gameObject, eventData);
    }

    public override void OnSubmit(BaseEventData eventData) {
        if(onSubmit != null)
            onSubmit(this.gameObject, eventData);
    }

    public override void OnUpdateSelected(BaseEventData eventData) {
        if(onUpdateSelected != null)
            onUpdateSelected(this.gameObject, eventData);
    }
}
