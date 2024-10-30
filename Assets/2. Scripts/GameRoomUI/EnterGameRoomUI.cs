using UnityEngine;
using TMPro;
using TCP_Enum;
using UnityEngine.UI;

public class EnterGameRoomUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField input_Password;
    [SerializeField] private Button btn_GameJoin;

    private RoomData selectRoom;

    private void OnEnable()
    {
        btn_GameJoin.onClick.AddListener(TryJoinGame);
    }

    private void OnDisable()
    {
        btn_GameJoin.onClick.RemoveListener(TryJoinGame);
    }

    //private void OnEnable()
    //{
    //    EventManager<Tcp_Room_Command>.Binding<RoomData>(true, Tcp_Room_Command.SelectRoom, SelectGameRoom);
    //}

    //private void OnDisable()
    //{
    //    EventManager<Tcp_Room_Command>.Binding<RoomData>(false, Tcp_Room_Command.SelectRoom, SelectGameRoom);
    //}

    public void SelectGameRoom(RoomData roomData)
    {
        selectRoom = roomData;
        Debug.LogError(selectRoom.ip);
    }

    private void TryJoinGame()
    {
        string password = string.IsNullOrEmpty(input_Password.text)? "Null": input_Password.text;

        string selectIp = selectRoom.ip;

        Debug.Log($"IP : {selectIp}");
        string selectPort = selectRoom.id;

        EventManager<Tcp_Room_Command>.TriggerEvent(Tcp_Room_Command.enterRoom, selectIp, selectPort, password);
    }
}
