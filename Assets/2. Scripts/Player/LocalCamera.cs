using Mirror;
using Cinemachine;
using UnityEngine;

public class LocalCamera : NetworkBehaviour
{
    [SerializeField] private Transform cameraArm;

    private CinemachineVirtualCamera camera;

    private void Awake()
    {
        camera = GetComponent<CinemachineVirtualCamera>();

        camera.enabled = false;
    }

    private void Start()
    {
        if (isLocalPlayer)
        {
            camera.enabled = true;
            camera.Follow = cameraArm;
        }
    }
}
