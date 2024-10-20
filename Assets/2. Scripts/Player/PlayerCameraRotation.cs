using PlayerEventEnum;
using UnityEngine;

public class PlayerCameraRotation : MonoBehaviour
{
    [SerializeField] private Transform _cameraArm;

    private void Awake()
    {
        AddEvent();
    }

    private void OnDestroy()
    {
        RemoveEvent();
    }

    private void Start()
    {
        var xAngle = _cameraArm.eulerAngles.x;
        var yAngle = _cameraArm.eulerAngles.y;

        EventManager<Init>.TriggerEvent(Init.InitCameraRotationValue, xAngle, yAngle);
    }

    private void AddEvent()
    {
        EventManager<PlayerController>.Binding<float, float>(true, PlayerController.SetCameraRotation, CameraRotate);
    }

    private void RemoveEvent()
    {
        EventManager<PlayerController>.Binding<float, float>(false, PlayerController.SetCameraRotation, CameraRotate);
    }

    private void CameraRotate(float xAngle, float yAngle)
    {
        _cameraArm.rotation = Quaternion.Euler(xAngle, yAngle, 0);
    }
}
