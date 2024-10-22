using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using PlayerEventEnum;
using TCP_Enum;

public class CreateRoomUI : MonoBehaviour
{
    [SerializeField] private Button btn_CreateServer;

    private void Awake()
    {
        btn_CreateServer.onClick.AddListener(CreateRoom);
    }

    private void CreateRoom()
    {
        var manager = MyGameRoomManager.singleton;
        // 방 설정 작업 처리

        // 호스트가 방을 생성
        manager.StartHost();

        // 호스트의 IP 주소를 중앙 서버나 관리 스크립트에 등록
        string hostIP = NetworkManager.singleton.networkAddress;
        string roomID = "261212";

        // 디버깅
        string passward = "1234";

        // TCP 통신을 통해 DB에 방 생성 요청
        MyTCPClient.Instance.SendRequestToServer($"{Tcp_Room_Command.createRoom},{roomID},{hostIP},{passward}");  
    }

    private void Debug_PrintRoomList()
    {
        // TCP 통신을 통해 DB에 방 생성 요청
        MyTCPClient.Instance.SendRequestToServer($"{Tcp_Room_Command.getRoomList}");
    }

    //// 디버깅
    //private void OnDestroy()
    //{
    //    // 호스트의 IP 주소를 중앙 서버나 관리 스크립트에 등록
    //    string hostIP = NetworkManager.singleton.networkAddress;
    //    string roomID = "261212";

    //    // 디버깅
    //    string passward = "1234";

    //    // TCp 통신을 통해 방 제거 요청
    //    MyTCPClient.Instance.SendRequestToServer($"{Tcp_Room_Command.removeRoom},{roomID}");
    //}
}
