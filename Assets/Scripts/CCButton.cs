using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.Events;
using System;

public class CCButton : MonoBehaviour, IPointerClickHandler
{
    [Serializable]
    public class LeftButtonClickedEvent : UnityEvent { }

    [FormerlySerializedAs("onLeftClick"), SerializeField]
    private LeftButtonClickedEvent m_OnLeftClick = new LeftButtonClickedEvent();

    [Serializable]
    public class RightButtonClickedEvent : UnityEvent { }

    [FormerlySerializedAs("onRightClick"), SerializeField]
    private RightButtonClickedEvent m_OnRightClick = new RightButtonClickedEvent();

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                m_OnLeftClick.Invoke();
                break;
            case PointerEventData.InputButton.Right:
                m_OnRightClick.Invoke();
                break;
            case PointerEventData.InputButton.Middle:
                //中クリックの時の処理、はないです
                break;
            default:
                break;
        }
    }
}