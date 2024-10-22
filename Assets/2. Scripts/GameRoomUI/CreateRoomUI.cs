using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TCP_Enum;
using kcp2k;

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

        // 호스트의 IP 주소를 중앙 서버나 관리 스크립트에 등록
        // 게임 룸 서버 주소 Ip
        string hostIP = NetworkManager.singleton.networkAddress;
        // 게임 룸 포트
        var kcpTransport = manager.transport.GetComponent<KcpTransport>();
        kcpTransport.port = (ushort)Random.Range(1000, 9900);
        string hostPort = kcpTransport.port.ToString();
        // 게임 룸 Password

        // 디버깅
        string passward = "1234";

        // TCP 통신을 통해 DB에 방 생성 요청
        MyTCPClient.Instance.SendRequestToServer($"{Tcp_Room_Command.createRoom},{hostPort},{hostIP},{passward}");

        // 호스트가 방을 생성
        manager.StartHost();
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
