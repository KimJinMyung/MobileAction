using PlayerEventEnum;
using UnityEngine;
using UnityEngine.EventSystems;

public class DriftButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        EventManager<PlayerController>.TriggerEvent(PlayerController.Drift, true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        EventManager<PlayerController>.TriggerEvent(PlayerController.Drift, false);
    }
}
