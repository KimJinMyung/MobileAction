namespace PlayerEventEnum
{
    public enum PlayerController
    {
        ForwardMove,
        LeftMove,
        SetJoyStickActive,
        SetJoyStickBackPosition,
        SetJoyStickDrag,
        Drift,
        Boost
    }

    public enum GameRoomPlayer
    {
        ConnectPlayer,
        DisConnectPlayer,
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

    public enum UserData
    {
        ChangedUser,
    }
}