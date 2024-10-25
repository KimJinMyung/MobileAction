using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TCP_Enum;
using UnityEngine;
using UnityEngine.UI;

public class GameRoomRobbyUI : MonoBehaviour
{
    [SerializeField] private GameObject gameRoomButton;
    [SerializeField] private Transform gameRoomListContent;

    [SerializeField] private Button btn_RefreshGameRoomList;

    private void Awake()
    {
        AddEvent();

        btn_RefreshGameRoomList.onClick.AddListener(RequestUpdateRoomData);
    }

    private void OnDestroy()
    {
        RemoveEvent();

        btn_RefreshGameRoomList.onClick.RemoveListener(RequestUpdateRoomData);
    }

    private void AddEvent()
    {
        EventManager<Tcp_Room_Command>.Binding(true, Tcp_Room_Command.requestRoomList, RequestUpdateRoomData);
        EventManager<Tcp_Room_Command>.Binding<string>(true, Tcp_Room_Command.UpdateRoomList, UpdateRoomList);
    }

    private void RemoveEvent()
    {
        EventManager<Tcp_Room_Command>.Binding(false, Tcp_Room_Command.requestRoomList, RequestUpdateRoomData);
        EventManager<Tcp_Room_Command>.Binding<string>(false, Tcp_Room_Command.UpdateRoomList, UpdateRoomList);
    }

    private async void RequestUpdateRoomData()
    {
        // 방 목록 요청
        MyTCPClient.Instance.SendRequestToServer($"{Tcp_Room_Command.getRoomList}");

        // 방 목록 요청 후 서버 응답 기다림
        var roomListData = await MyTCPClient.Instance.ReceiveRoomListFromServer(Tcp_Room_Command.getRoomList);

        UpdateRoomList(roomListData);
    }

    // port, ip, password, playerCount
    private void UpdateRoomList(string roomListData)
    {
        EventManager<Tcp_Room_Command>.TriggerEvent(Tcp_Room_Command.getRoomList, roomListData, gameRoomButton, gameRoomListContent);
    }
}
