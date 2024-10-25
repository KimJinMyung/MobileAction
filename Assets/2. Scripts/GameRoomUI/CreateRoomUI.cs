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
    //    // TCP ����� ���� DB�� �� ���� ��û
    //    MyTCPClient.Instance.SendRequestToServer($"{Tcp_Room_Command.getRoomList}");
    //}

    //// �����
    //private void OnDestroy()
    //{
    //    // ȣ��Ʈ�� IP �ּҸ� �߾� ������ ���� ��ũ��Ʈ�� ���
    //    string hostIP = NetworkManager.singleton.networkAddress;
    //    string roomID = "261212";

    //    // �����
    //    string passward = "1234";

    //    // TCp ����� ���� �� ���� ��û
    //    MyTCPClient.Instance.SendRequestToServer($"{Tcp_Room_Command.removeRoom},{roomID}");
    //}
}
