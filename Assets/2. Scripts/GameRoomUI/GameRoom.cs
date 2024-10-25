using UnityEngine;
using UnityEngine.UI;
using TCP_Enum;
using TMPro;

public class GameRoom : MonoBehaviour
{
    [SerializeField] private TMP_Text RoomName;
    [SerializeField] private TMP_Text PlayerCount;
    [SerializeField] private Image LockOnIcon;

    public RoomData roomData { get; private set; }
    private Button btn_GameRoom;

    // µð¹ö±ë
    //public string serverIPAddress = "127.0.0.1";

    private void Awake()
    {
        btn_GameRoom = GetComponent<Button>();

        btn_GameRoom.onClick.AddListener(EnterGameRoom);
    }

    public void InitGameRoomData(RoomData roomData)
    {
        this.roomData = roomData;

        RoomName.text = roomData.roomName;
        PlayerCount.text = $"{roomData.currentPlayerCount} / {roomData.maxPlayerCount}";
        LockOnIcon.enabled = roomData.isLock;
    }

    private void EnterGameRoom()
    {
        //EventManager<Tcp_Room_Command>.TriggerEvent(Tcp_Room_Command.enterRoom, roomID, roomIP);
        EventManager<Tcp_Room_Command>.TriggerEvent(Tcp_Room_Command.SelectRoom, roomData);
    }
}
