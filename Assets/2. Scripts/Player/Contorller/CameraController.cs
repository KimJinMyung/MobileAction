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
        // ī�޶��� ���� ȸ�� ���� ������ ��������
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
        // Time.deltaTime ����, ��ũ�� �ػ� ������ ����
        yAngle = yAngleTemp + (dragPos.x - beginPos.x) * rotationSpeed;
        xAngle = xAngleTemp - (dragPos.y - beginPos.y) * rotationSpeed;

        // X�� ȸ�� ���� ����
        xAngle = Mathf.Clamp(xAngle, -60f, 30f);

        // ī�޶� ȸ�� ����
        EventManager<PlayerController>.TriggerEvent(PlayerController.SetCameraRotation, xAngle, yAngle);
    }
}
