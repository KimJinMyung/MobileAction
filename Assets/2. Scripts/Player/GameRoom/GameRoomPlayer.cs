using UnityEngine;
using Mirror;
using PlayerEventEnum;
using TCP_Enum;

public class GameRoomPlayer : NetworkRoomPlayer
{
    [SerializeField] private GameObject gameRoomPlayerRawImage;
    [SerializeField] private Camera camera;
    public Camera Cam { get { return camera; } }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!isLocalPlayer) return;

        // 캔버스의 connectPlayer 메서드 실행
        EventManager<PlayerEventEnum.GameRoomPlayer>.TriggerEvent(PlayerEventEnum.GameRoomPlayer.ConnectPlayer, this);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        if (!isLocalPlayer) return;

        // 캔버스의 disConnectPlayer 메서드 실행
        EventManager<PlayerEventEnum.GameRoomPlayer>.TriggerEvent(PlayerEventEnum.GameRoomPlayer.DisConnectPlayer, this);

        // TCP 서버에서 추방되었다고 전달
        //MyTCPClient.Instance.SendRequestToServer($"{Tcp_Room_Command.getRoomList}");

        EventManager<Tcp_Room_Command>.TriggerEvent(Tcp_Room_Command.ChangedPlayerCount, -1);
    }
}
