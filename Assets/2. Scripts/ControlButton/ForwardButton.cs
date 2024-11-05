using PlayerEventEnum;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ForwardButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Button btn_Gear;

    private bool isForward;

    private void Awake()
    {
        btn_Gear.onClick.AddListener(ChangedForwardGear);
    }

    private void Start()
    {
        isForward = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        float forward = isForward ? 1f : -1f;
        EventManager<PlayerController>.TriggerEvent(PlayerController.ForwardMove, forward);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        EventManager<PlayerController>.TriggerEvent(PlayerController.ForwardMove, 0f);
    }

    private void ChangedForwardGear()
    {
        isForward = !isForward;
    }
}
