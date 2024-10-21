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
        SpawnPlayCharacter,
    }

    public enum PlayerHUDEvent
    {
        SpawnPlayer,
        DestroyPlayer,
        EnableHUD,
    }

    public enum DB_Event
    {
        CreateRoom,
        DeleteRoom,
        EnterRoom,
    }
}