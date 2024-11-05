using Mirror;
using PlayerEventEnum;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private Transform _cameraArm;
    [SerializeField] private Transform _playerMesh;

    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 10f;

    private Rigidbody rb;

    private Vector2 movement;
    private float moveSpeed;
    private bool isRunning;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        AddEvent();
    }

    private void OnDestroy()
    {
        RemoveEvent();
    }

    private void AddEvent()
    {
        EventManager<PlayerController>.Binding<Vector2>(true, PlayerController.ForwardMove, OnMovement);
    }

    private void RemoveEvent()
    {
        EventManager<PlayerController>.Binding<Vector2>(false, PlayerController.ForwardMove, OnMovement);
    }

    private void OnMovement(Vector2 move)
    {
        if(isLocalPlayer)
            movement = move;
    }

    private void Move()
    {
        var moveDir = new Vector3(movement.x, 0, movement.y);
        if (moveDir.magnitude >= 0.1f)
        {
            var moveAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg + _cameraArm.transform.eulerAngles.y;
            var targetRotation = Quaternion.Euler(0, moveAngle, 0);
            var dir = Quaternion.Euler(0, moveAngle, 0) * Vector3.forward;

            // 부드럽게 회전 (Slerp를 사용하여 서서히 목표 회전각으로 회전)
            _playerMesh.rotation = Quaternion.Slerp(_playerMesh.rotation, targetRotation, Time.deltaTime * _rotationSpeed);

            if (!isRunning) moveSpeed = _moveSpeed;
            else moveSpeed = _moveSpeed * 1.5f;

            rb.velocity = dir * moveSpeed;
        }
        else
        {
            rb.velocity = Vector3.zero;
        }

        var velocityMagnitude = rb.velocity.magnitude;        
    }

    private void FixedUpdate()
    {
        Move();
    }
}
