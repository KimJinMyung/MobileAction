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

    // 디버깅
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

        // 게임룸 서버의 IP 주소를 설정
        //manager.networkAddress = serverIPAddress;

        // 클라이언트 시작
        manager.StartClient();
    }
}
