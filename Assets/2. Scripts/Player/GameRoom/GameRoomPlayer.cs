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

        // ĵ������ connectPlayer �޼��� ����
        EventManager<PlayerEventEnum.GameRoomPlayer>.TriggerEvent(PlayerEventEnum.GameRoomPlayer.ConnectPlayer, this);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        if (!isLocalPlayer) return;

        // ĵ������ disConnectPlayer �޼��� ����
        EventManager<PlayerEventEnum.GameRoomPlayer>.TriggerEvent(PlayerEventEnum.GameRoomPlayer.DisConnectPlayer, this);

        // TCP �������� �߹�Ǿ��ٰ� ����
        //MyTCPClient.Instance.SendRequestToServer($"{Tcp_Room_Command.getRoomList}");

        EventManager<Tcp_Room_Command>.TriggerEvent(Tcp_Room_Command.ChangedPlayerCount, -1);
    }
}
