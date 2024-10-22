using System;
using System.Collections;
using System.Collections.Generic;
using TCP_Enum;
using UnityEngine;

public class GameRoomRobbyUI : MonoBehaviour
{
    [SerializeField] private GameObject GameRoomButton;

    private void Awake()
    {
        AddEvent();        
    }

    private void OnDestroy()
    {
        RemoveEvent();
    }

    private void AddEvent()
    {
        EventManager<Tcp_Room_Command>.Binding(true, Tcp_Room_Command.requestRoomList, RequestUpdateRoomData);
        EventManager<Tcp_Room_Command>.Binding<string>(true, Tcp_Room_Command.getRoomList, UpdateRoomList);
    }

    private void RemoveEvent()
    {
        EventManager<Tcp_Room_Command>.Binding(false, Tcp_Room_Command.requestRoomList, RequestUpdateRoomData);
        EventManager<Tcp_Room_Command>.Binding<string>(false, Tcp_Room_Command.getRoomList, UpdateRoomList);
    }

    private void RequestUpdateRoomData()
    {
        // 방 목록 요청
        MyTCPClient.Instance.SendRequestToServer($"{Tcp_Room_Command.getRoomList}");

        // 방 목록 요청 후 서버 응답 기다림
        MyTCPClient.Instance.ReceiveRoomListFromServer(Tcp_Room_Command.getRoomList);
    }

    private void UpdateRoomList(string roomListData)
    {
        string[] rooms = roomListData.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        foreach(var room in rooms)
        {
            if(string.IsNullOrEmpty(room)) continue;

            string[] roomdatas = room.Split(',');

            RoomData roomdata = new RoomData();
            roomdata.id = roomdatas[0];
            roomdata.ip = roomdatas[1];
            //roomdata.password = roomdatas[2];            

            //GameObject newGameRoom = Instantiate(GameRoomButton);
            //EnterGameRoomUI GameRoomData = newGameRoom.GetComponent<EnterGameRoomUI>();
            //GameRoomData.InitGameRoomData()
        }
    }
}
