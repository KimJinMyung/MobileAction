using System.Collections;
using System.Collections.Generic;
using TCP_Enum;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameLobbyCanvasManager : MonoBehaviour
{
    [SerializeField] private Button btn_CreateRoom;
    [SerializeField] private Button btn_JoinRoom;

    [SerializeField] private GameObject popUp_CreateRoom;
    [SerializeField] private GameObject popUp_JoinRoom;
    [SerializeField] private GameObject popUp_EnterRoom;

    private void OnEnable()
    {
        btn_CreateRoom.onClick.AddListener(PopUpCreateRoom);
        btn_JoinRoom.onClick.AddListener(PopUpJoinRoom);

        EventManager<Tcp_Room_Command>.Binding<RoomData>(true, Tcp_Room_Command.SelectRoom, PopUpEnterGameRoom);
    }

    private void OnDisable()
    {
        btn_CreateRoom.onClick.RemoveListener(PopUpCreateRoom);
        btn_JoinRoom.onClick.RemoveListener(PopUpJoinRoom);

        EventManager<Tcp_Room_Command>.Binding<RoomData>(false, Tcp_Room_Command.SelectRoom, PopUpEnterGameRoom);
    }

    private void PopUpCreateRoom()
    {
        popUp_CreateRoom.SetActive(true);
        popUp_JoinRoom.SetActive(false);
        popUp_EnterRoom.SetActive(false);
    }

    private void PopUpJoinRoom()
    {
        popUp_CreateRoom.SetActive(false);
        popUp_JoinRoom.SetActive(true);
        popUp_EnterRoom.SetActive(false);
    }

    private void PopUpEnterGameRoom(RoomData roomData)
    {
        if (roomData.isLock)
        {
            popUp_CreateRoom.SetActive(false);
            popUp_JoinRoom.SetActive(false);
            popUp_EnterRoom.SetActive(true);

            popUp_EnterRoom.GetComponent<EnterGameRoomUI>().SelectGameRoom(roomData);
        }
        else
        {
            EventManager<Tcp_Room_Command>.TriggerEvent(Tcp_Room_Command.enterRoom, roomData.ip, roomData.id, "Null");
        }       
    }
}
