using Cinemachine;
using Mirror;
using PlayerEventEnum;
using UnityEngine;
using System.Collections;

public enum PlayerState
{
    Normal,
    isDrift,
    isBoost,
}

public class Movement : NetworkBehaviour
{
    [Header("SpeedValue")]
    [SerializeField] private float moveSpeed = 10f; // 가속도
    [SerializeField] private float maxMoveSpeed = 30f;
    [SerializeField] private float rotateSpeed = 50f;
    [SerializeField] private float driftSpeedMultiplier = 0.8f;
    [SerializeField] private float driftTurnSpeed = 100f;
    [SerializeField] private float boostSpeedMultiplier = 1.5f;
    [SerializeField] private float boostMaxMoveSpeed = 50f;
    [SerializeField] private float boostTurnSpeed = 25f;

    [Header("physic Material")]
    [SerializeField] private PhysicMaterial defaultMaterial;
    [SerializeField] private PhysicMaterial driftMaterial;

    [Header("Booster Duration Time")]
    [SerializeField] private float durationTime = 100f;
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
        EventManager<PlayerController>.Binding(true, PlayerController.Boost, StartBoost);
    }

    private void RemoveEvent()
    {
        EventManager<PlayerController>.Binding<float>(false, PlayerController.ForwardMove, InputForwardMovement);
        EventManager<PlayerController>.Binding<float>(false, PlayerController.LeftMove, InputRotateMovement);
        EventManager<PlayerController>.Binding<bool>(false, PlayerController.Drift, InputDrift);
        EventManager<PlayerController>.Binding(false, PlayerController.Boost, StartBoost);
    }

    public void InputForwardMovement(float isForward)
    {
        if (isLocalPlayer) /*inputMove = move;*/
            inputForwardMove = isForward;

        Debug.Log("이동 중...");
    }

    public void InputRotateMovement(float isLeft)
    {
        if (isLocalPlayer) inputRotateMove = isLeft;
    }

    public void InputDrift(bool isDrift)
    {
        if (!isLocalPlayer) return;

        state = isDrift ? PlayerState.isDrift : PlayerState.Normal;
        physicMaterial = isDrift ? driftMaterial : defaultMaterial;
    }

    public void InputBooster(bool isBoost)
    {
        if (!isLocalPlayer) return;

        state = isBoost ? PlayerState.isBoost : PlayerState.Normal;
        physicMaterial = isBoost ? driftMaterial : defaultMaterial;
    }

    private void MoveForward()
    {
        float forwardMoveDir = inputForwardMove;    //inputMove.y;
        if (Mathf.Abs(forwardMoveDir) > 0.1f)
        {
            var moveDir = transform.forward * forwardMoveDir * (state == PlayerState.isDrift? driftSpeedMultiplier : 1f);
            var speed = state == PlayerState.isBoost ? moveSpeed * boostSpeedMultiplier : moveSpeed;
            rb.AddForce(moveDir * speed, ForceMode.Force);

            // 최고 속도 제한
            var LimitSpeed = state == PlayerState.isBoost ? boostMaxMoveSpeed : maxMoveSpeed;

            if (rb.velocity.magnitude >= LimitSpeed)
            {
                rb.velocity = rb.velocity.normalized * LimitSpeed;
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

    private void StartBoost()
    {
        InputBooster(true);

        StartCoroutine(CheckBoostDuration());
    }

    IEnumerator CheckBoostDuration()
    {
        yield return new WaitForSeconds(durationTime);

        InputBooster(false);
    }

    private void Update()
    {
        MoveForward();
        RotateCar();
    }
}
