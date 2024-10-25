using UnityEngine;
using UnityEngine.UI;
using TCP_Enum;
using TMPro;

public class CreateRoomUI : MonoBehaviour
{
    [SerializeField] private Button btn_CreateServer;
    [SerializeField] private TMP_InputField input_RoomName;

    [SerializeField] private TMP_Text text_PlayerCount;
    [SerializeField] private Button btn_IncreaseCount;
    [SerializeField] private Button btn_DecreaseCount;

    [SerializeField] private TMP_InputField input_Password;

    private int maxPlayerCount = 5;

    private void Awake()
    {
        btn_CreateServer.onClick.AddListener(CreateRoom);
        btn_IncreaseCount.onClick.AddListener(IncreaseCount);
        btn_DecreaseCount.onClick.AddListener(DecreaseCount);
    }

    private void OnDestroy()
    {
        btn_CreateServer.onClick.RemoveListener(CreateRoom);
        btn_IncreaseCount.onClick.RemoveListener(IncreaseCount);
        btn_DecreaseCount.onClick.RemoveListener(DecreaseCount);
    }

    private void Start()
    {
        text_PlayerCount.text = maxPlayerCount.ToString();
    }

    private void CreateRoom()
    {
        EventManager<Tcp_Room_Command>.TriggerEvent(Tcp_Room_Command.createRoom, input_RoomName.text, input_Password.text, maxPlayerCount);
    }

    private void IncreaseCount()
    {
        maxPlayerCount++;
        text_PlayerCount.text = maxPlayerCount.ToString();
    }

    private void DecreaseCount()
    {
        maxPlayerCount--;
        text_PlayerCount.text = maxPlayerCount.ToString();
    }

    //private void Debug_PrintRoomList()
    //{
    //    // TCP 통신을 통해 DB에 방 생성 요청
    //    MyTCPClient.Instance.SendRequestToServer($"{Tcp_Room_Command.getRoomList}");
    //}

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
