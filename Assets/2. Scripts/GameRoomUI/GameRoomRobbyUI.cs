using System;
using System.Collections;
using System.Collections.Generic;
using TCP_Enum;
using UnityEngine;

public class GameRoomRobbyUI : MonoBehaviour
{
    [SerializeField] private GameObject GameRoomButton;
    [SerializeField] private Transform GameRoomListContent;

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
        // �� ��� ��û
        MyTCPClient.Instance.SendRequestToServer($"{Tcp_Room_Command.getRoomList}");

        // �� ��� ��û �� ���� ���� ��ٸ�
        MyTCPClient.Instance.ReceiveRoomListFromServer(Tcp_Room_Command.getRoomList);
    }

    // port, ip, password, playerCount
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
            roomdata.password = roomdatas[2];
            roomdata.roomName = roomdatas[3];
            roomdata.currentPlayerCount = int.Parse(roomdatas[4]);
            roomdata.maxPlayerCount = int.Parse(roomdatas[5]);

            GameObject newGameRoom = Instantiate(GameRoomButton);
            newGameRoom.transform.parent = GameRoomListContent;
            EnterGameRoomUI GameRoomData = newGameRoom.GetComponent<EnterGameRoomUI>();
            GameRoomData.InitGameRoomData(roomdata);
        }
    }
}
