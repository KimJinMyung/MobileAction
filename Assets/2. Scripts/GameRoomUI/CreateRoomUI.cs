using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using PlayerEventEnum;

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
        string roomID = NetworkManager.singleton.GetInstanceID().ToString();

        // 디버깅
        string passward = "0000";

        EventManager<DB_Event>.TriggerEvent(DB_Event.CreateRoom, hostIP, roomID, passward);


        // 디버깅
        Debug.Log(hostIP);
    }
}
