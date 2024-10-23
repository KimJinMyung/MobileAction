using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TCP_Enum;
using TMPro;

public class EnterGameRoomUI : MonoBehaviour
{
    [SerializeField] private TMP_Text RoomName;
    [SerializeField] private TMP_Text PlayerCount;
    [SerializeField] private Image LockOnIcon;

    private RoomData roomData;
    private Button btn_GameRoom;

    // �����
    //public string serverIPAddress = "127.0.0.1";

    private void Awake()
    {
        btn_GameRoom = GetComponent<Button>();



        //btn_GameRoom.onClick.AddListener(EnterGameRoom);
    }

    public void InitGameRoomData(RoomData roomData)
    {
        this.roomData = roomData;

        RoomName.text = roomData.roomName;
        PlayerCount.text = $"{roomData.currentPlayerCount} / {roomData.maxPlayerCount}";
        LockOnIcon.enabled = int.Parse(roomData.password) != 1234;
    }

    private void EnterGameRoom()
    {
        var manager = MyGameRoomManager.singleton;

        // ���ӷ� ������ IP �ּҸ� ����
        //manager.networkAddress = serverIPAddress;

        // Ŭ���̾�Ʈ ����
        manager.StartClient();
    }
}
