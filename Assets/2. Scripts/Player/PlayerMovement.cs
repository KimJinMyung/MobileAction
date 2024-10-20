using UnityEngine;

public class PlayerMovement : MonoBehaviour
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
    }

    private void DebugPressButton()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            movement += new Vector2(-1, 0);
        }
        
        if (Input.GetKeyUp(KeyCode.A))
        {
            movement -= new Vector2(-1, 0);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            movement += new Vector2(1, 0);
        }        
        
        if (Input.GetKeyUp(KeyCode.D))
        {
            movement -= new Vector2(1, 0);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            movement += new Vector2(0, 1);
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            movement -= new Vector2(0, 1);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            movement += new Vector2(0, -1);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            movement -= new Vector2(0, -1);
        }
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

    private void Update()
    {
        DebugPressButton();
    }

    private void FixedUpdate()
    {
        Move();
    }
}
