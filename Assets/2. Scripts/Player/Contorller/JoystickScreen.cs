using PlayerEventEnum;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickScreen : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public void OnDrag(PointerEventData eventData)
    {
        EventManager<PlayerController>.TriggerEvent(PlayerController.SetJoyStickDrag, eventData.position);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        EventManager<PlayerController>.TriggerEvent(PlayerController.SetJoyStickActive, true);

        EventManager<PlayerController>.TriggerEvent(PlayerController.SetJoyStickBackPosition, eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        EventManager<PlayerController>.TriggerEvent(PlayerController.SetJoyStickActive, false);

        EventManager<PlayerController>.TriggerEvent(PlayerController.ForwardMove, Vector2.zero);
    }
}
