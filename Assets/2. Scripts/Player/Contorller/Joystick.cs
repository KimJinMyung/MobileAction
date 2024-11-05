using PlayerEventEnum;
using UnityEngine;
using UnityEngine.UI;

public class Joystick : MonoBehaviour
{
    [SerializeField] private RectTransform _joystick;
    [SerializeField] private Image Img_joystick;
    [SerializeField] private Canvas Canvas_joystick;

    private RectTransform rectTransform;
    private Image joyStickBackImg;
    private float joystickRadius;

    private bool isController;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        joyStickBackImg = GetComponent<Image>();

        AddEvent();
    }

    private void OnDestroy()
    {
        RemoveEvent();
    }

    private void Start()
    {
        joystickRadius = rectTransform.rect.width * 0.5f - _joystick.rect.width * 0.5f;

        isController = false;

        joyStickBackImg.enabled = isController;
        Img_joystick.enabled = isController;
    }

    private void AddEvent()
    {
        EventManager<PlayerController>.Binding<bool>(true, PlayerController.SetJoyStickActive, SetJoyStickActive);
        EventManager<PlayerController>.Binding<Vector2>(true, PlayerController.SetJoyStickBackPosition, SetJoyStickBackPosition);
        EventManager<PlayerController>.Binding<Vector2>(true, PlayerController.SetJoyStickDrag, OnTouch);
    }

    private void RemoveEvent()
    {
        EventManager<PlayerController>.Binding<bool>(false, PlayerController.SetJoyStickActive, SetJoyStickActive);
        EventManager<PlayerController>.Binding<Vector2>(false, PlayerController.SetJoyStickBackPosition, SetJoyStickBackPosition);
        EventManager<PlayerController>.Binding<Vector2>(false, PlayerController.SetJoyStickDrag, OnTouch);
    }

    private void OnTouch(Vector2 touch)
    {
        var inputPos = touch - (Vector2)rectTransform.position;

        var rangDir = inputPos.magnitude < joystickRadius ? inputPos : inputPos.normalized * joystickRadius;
        _joystick.anchoredPosition = rangDir;

        EventManager<PlayerController>.TriggerEvent(PlayerController.ForwardMove, rangDir.normalized);
    }

    private void SetJoyStickActive(bool setController)
    {
        if (isController == setController) return;

        this.isController = setController;

        joyStickBackImg.enabled = isController;
        Img_joystick.enabled = isController;

        if (!isController) _joystick.anchoredPosition = Vector2.zero;
    }

    private void SetJoyStickBackPosition(Vector2 joystickBackPos)
    {
        var position = joystickBackPos;
        transform.position = joystickBackPos;
    }
}
