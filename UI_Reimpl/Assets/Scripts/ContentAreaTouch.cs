using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace UIReimpl
{
    public enum ContentAreaEventType
    {
        OnPointerUp,
        OnPointerDown,
        OnPointerClick,
        OnDrag,
        OnBeginDrag,
        OnEndDrag
    }

    public static class ContentAreaEventData
    {
        public static string[] TYPE_STRS = { "OnPointerUp", "OnPointerDown",
        "OnPointerClick", "OnDrag", "OnBeginDrag", "OnEndDrag"};
    }

    public class ContentAreaEvent : CustomUIEvent
    {
        public ContentAreaEventType content_event_type;
        public PointerEventData content_pointer_data;

        public ContentAreaEvent(ContentAreaEventType type, PointerEventData data)
        {
            content_event_type = type;
            content_pointer_data = data;
        }

        public override void process_debug(UIDebugWindowA win1)
        {
            switch (content_event_type)
            {
                case ContentAreaEventType.OnPointerUp:
                    win1.text_comp.text = ContentAreaEventData.TYPE_STRS[0];
                    Debug.Log($"{ContentAreaEventData.TYPE_STRS[0]}");
                    break;
                case ContentAreaEventType.OnPointerDown:
                    win1.text_comp.text = ContentAreaEventData.TYPE_STRS[1];
                    Debug.Log($"{ContentAreaEventData.TYPE_STRS[1]}");
                    break;
                case ContentAreaEventType.OnPointerClick:
                    win1.text_comp.text = ContentAreaEventData.TYPE_STRS[2];
                    Debug.Log($"{ContentAreaEventData.TYPE_STRS[2]}");
                    break;
                case ContentAreaEventType.OnDrag:
                    win1.text_comp.text = ContentAreaEventData.TYPE_STRS[3];
                    Debug.Log($"{ContentAreaEventData.TYPE_STRS[3]}");
                    break;
                case ContentAreaEventType.OnBeginDrag:
                    win1.text_comp.text = ContentAreaEventData.TYPE_STRS[4];
                    Debug.Log($"{ContentAreaEventData.TYPE_STRS[4]}");
                    break;
                case ContentAreaEventType.OnEndDrag:
                    win1.text_comp.text = ContentAreaEventData.TYPE_STRS[5];
                    Debug.Log($"{ContentAreaEventData.TYPE_STRS[5]}");
                    break;
            }
        }

        public override void process_release()
        {
            process_debug(null);
        }

    }


    public class ContentAreaTouch : MonoBehaviour, IPointerDownHandler,
    IPointerUpHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public UIEventMgr event_mgr;

        public void OnPointerUp(PointerEventData eventData)
        {
            event_mgr.AddEvent(new ContentAreaEvent(ContentAreaEventType.OnPointerUp, eventData));
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            event_mgr.AddEvent(new ContentAreaEvent(ContentAreaEventType.OnPointerDown, eventData));

        }

        public void OnPointerClick(PointerEventData eventData)
        {
            event_mgr.AddEvent(new ContentAreaEvent(ContentAreaEventType.OnPointerClick, eventData));
        }

        public void OnDrag(PointerEventData eventData)
        {
            event_mgr.AddEvent(new ContentAreaEvent(ContentAreaEventType.OnDrag, eventData));
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            event_mgr.AddEvent(new ContentAreaEvent(ContentAreaEventType.OnBeginDrag, eventData));
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            event_mgr.AddEvent(new ContentAreaEvent(ContentAreaEventType.OnEndDrag, eventData));
        }
    }

}
