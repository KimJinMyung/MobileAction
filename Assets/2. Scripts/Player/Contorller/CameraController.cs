using Cinemachine;
using PlayerEventEnum;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [SerializeField] private float rotationSpeed = 0.4f;

    Vector3 beginPos;
    Vector3 dragPos;

    float xAngle;
    float yAngle;
    float xAngleTemp;
    float yAngleTemp;

    private void Awake()
    {
        AddEvent();
    }

    private void OnDestroy()
    {
        RemoveEvent();
    }

    private void AddEvent()
    {
        EventManager<Init>.Binding<float, float>(true, Init.InitCameraRotationValue, InitCameraRotationValue);
    }

    private void RemoveEvent()
    {
        EventManager<Init>.Binding<float, float>(false, Init.InitCameraRotationValue, InitCameraRotationValue);
    }

    private void InitCameraRotationValue(float x, float y)
    {
        // 카메라의 현재 회전 값을 각도로 가져오기
        xAngle = x;
        yAngle = y;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        beginPos = eventData.position;

        xAngleTemp = xAngle;
        yAngleTemp = yAngle;
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragPos = eventData.position;
        // Time.deltaTime 제거, 스크린 해상도 조정도 생략
        yAngle = yAngleTemp + (dragPos.x - beginPos.x) * rotationSpeed;
        xAngle = xAngleTemp - (dragPos.y - beginPos.y) * rotationSpeed;

        // X축 회전 각도 제한
        xAngle = Mathf.Clamp(xAngle, -60f, 30f);

        // 카메라 회전 적용
        EventManager<PlayerController>.TriggerEvent(PlayerController.SetCameraRotation, xAngle, yAngle);
    }
}
