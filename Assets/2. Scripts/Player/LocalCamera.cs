using Mirror;
using Cinemachine;
using UnityEngine;

public class LocalCamera : NetworkBehaviour
{
    [SerializeField] private Transform cameraArm;

    private CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();

        virtualCamera.enabled = false;
    }

    private void Start()
    {
        if (isLocalPlayer)
        {
            virtualCamera.enabled = true;
            virtualCamera.Follow = cameraArm;
        }
    }
}
