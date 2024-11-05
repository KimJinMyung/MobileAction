using PlayerEventEnum;
using UnityEngine;
using UnityEngine.EventSystems;

public class LeftRotateButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        EventManager<PlayerController>.TriggerEvent(PlayerController.LeftMove, 1f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        EventManager<PlayerController>.TriggerEvent(PlayerController.LeftMove, 0f);
    }
}
