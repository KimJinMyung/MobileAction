using PlayerEventEnum;
using UnityEngine;
using UnityEngine.UI;

public class BackMirrorUI : MonoBehaviour
{
    [SerializeField] private RawImage img_BackMirror;

    private Image backMirrorBackground;

    private void Awake()
    {
        backMirrorBackground = GetComponent<Image>();

        AddEvent();
    }

    private void OnDestroy()
    {
        RemoveEvent();
    }

    private void Start()
    {
        SetActive(false);
    }

    private void AddEvent()
    {
        EventManager<PlayerController>.Binding<bool>(true, PlayerController.ViewBackMirror, SetActive);
    }

    private void RemoveEvent()
    {
        EventManager<PlayerController>.Binding<bool>(false, PlayerController.ViewBackMirror, SetActive);
    }

    private void SetActive(bool isActive)
    {
        backMirrorBackground.enabled = isActive;
        img_BackMirror.enabled = isActive;
    }
}
