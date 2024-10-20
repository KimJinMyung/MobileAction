namespace PlayerEventEnum
{
    public enum PlayerController
    {
        Movement,
        SetJoyStickActive,
        SetJoyStickBackPosition,
        SetJoyStickDrag,
        SetCameraRotation,
    }

    public enum Init
    {
        InitCameraRotationValue,
    }

    public enum PlayerHUDEvent
    {
        SpawnPlayer,
        DestroyPlayer,
    }
}