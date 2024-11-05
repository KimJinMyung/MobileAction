using Cinemachine;
using Mirror;
using PlayerEventEnum;
using UnityEngine;

public enum PlayerState
{
    Normal,
    isDrift,
    isBoost,
}

public class Movement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 10f; // 가속도
    [SerializeField] private float maxMoveSpeed = 30f;
    [SerializeField] private float rotateSpeed = 50f;
    [SerializeField] private float driftSpeedMultiplier = 0.8f;
    [SerializeField] private float driftTurnSpeed = 100f;
    [SerializeField] private float boostSpeedMultiplier = 1.5f;
    [SerializeField] private float boostMaxMoveSpeed = 50f;
    [SerializeField] private float boostTurnSpeed = 25f;

    [SerializeField] private PhysicMaterial defaultMaterial;
    [SerializeField] private PhysicMaterial driftMaterial;
    [SerializeField] private PhysicMaterial boostMaterial;
    //[SerializeField] private CinemachineVirtualCamera cam;

    private Rigidbody rb;
    private PhysicMaterial physicMaterial;

    //private Vector3 inputMove;

    private float inputForwardMove;
    private float inputRotateMove;

    private PlayerState state;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        physicMaterial = GetComponent<Collider>().material;
    }

    private void OnEnable()
    {
        state = PlayerState.Normal;
        physicMaterial = defaultMaterial;
        inputForwardMove = 0f;

        AddEvent();
    }

    private void OnDisable()
    {
        RemoveEvent();
    }

    private void AddEvent()
    {
        EventManager<PlayerController>.Binding<float>(true, PlayerController.ForwardMove, InputForwardMovement);
        EventManager<PlayerController>.Binding<float>(true, PlayerController.LeftMove, InputRotateMovement);
        EventManager<PlayerController>.Binding<bool>(true, PlayerController.Drift, InputDrift);
        EventManager<PlayerController>.Binding<bool>(true, PlayerController.Boost, InputBooster);
    }

    private void RemoveEvent()
    {
        EventManager<PlayerController>.Binding<float>(false, PlayerController.ForwardMove, InputForwardMovement);
        EventManager<PlayerController>.Binding<float>(false, PlayerController.LeftMove, InputRotateMovement);
        EventManager<PlayerController>.Binding<bool>(false, PlayerController.Drift, InputDrift);
        EventManager<PlayerController>.Binding<bool>(false, PlayerController.Boost, InputBooster);
    }

    public void InputForwardMovement(float isForward)
    {
        if (isLocalPlayer) /*inputMove = move;*/
            inputForwardMove = isForward;
    }

    public void InputRotateMovement(float isLeft)
    {
        if (isLocalPlayer) inputRotateMove = isLeft;
    }

    public void InputDrift(bool isDrift)
    {
        state = isDrift ? PlayerState.isDrift : PlayerState.Normal;
        physicMaterial = isDrift ? driftMaterial : defaultMaterial;
    }

    public void InputBooster(bool isBoost)
    {
        state = isBoost ? PlayerState.isBoost : PlayerState.Normal;
        physicMaterial = isBoost ? boostMaterial : defaultMaterial;
    }

    private void MoveForward()
    {
        float forwardMoveDir = inputForwardMove;    //inputMove.y;
        if (Mathf.Abs(forwardMoveDir) > 0.1f)
        {
            var moveDir = transform.forward * forwardMoveDir * (state == PlayerState.isDrift? driftSpeedMultiplier : 1f);
            rb.AddForce(moveDir * moveSpeed, ForceMode.Force);

            // 최고 속도 제한
            if(rb.velocity.magnitude >= maxMoveSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxMoveSpeed;
            }
        }
    }

    private void RotateCar()
    {
        var rightMoveDir = inputRotateMove;     //inputMove.x;
        if (Mathf.Abs(rightMoveDir) > 0.1f)
        {
            // 차량 속도에 비례해 회전 속도 조정
            float adjustedRotateSpeed = (rb.velocity.magnitude / moveSpeed) * (state == PlayerState.isDrift ? driftTurnSpeed : rotateSpeed);
            transform.Rotate(Vector3.up, adjustedRotateSpeed * rightMoveDir * Time.deltaTime);
        }
    }

    private void Update()
    {
        MoveForward();
        RotateCar();
    }
}
