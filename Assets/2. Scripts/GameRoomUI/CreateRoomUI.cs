using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TCP_Enum;
using kcp2k;
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

    private void Start()
    {
        text_PlayerCount.text = maxPlayerCount.ToString();
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

    private void CreateRoom()
    {
        var manager = MyGameRoomManager.singleton;

        // �� ���� �۾� ó��
        
        // ȣ��Ʈ�� IP �ּҸ� �߾� ������ ���� ��ũ��Ʈ�� ���
        // ���� �� ���� �ּ� Ip
        string hostIP = NetworkManager.singleton.networkAddress;
        // ���� �� ��Ʈ
        var kcpTransport = manager.transport.GetComponent<KcpTransport>();

        // ���� ��
        kcpTransport.port = (ushort)Random.Range(1000, 9900);
        string hostPort = kcpTransport.port.ToString();

        string roomName = input_RoomName.text;

        // �ִ� �÷��̾� ��
        int maxPlayerCount = this.maxPlayerCount;

        // ���� �� Password
        string passward = string.IsNullOrWhiteSpace(input_Password.text)? "1234" : input_Password.text;

        string sendMessage = $"{Tcp_Room_Command.createRoom},{hostPort},{hostIP},{passward},{roomName},1,{maxPlayerCount}";
        // TCP ����� ���� DB�� �� ���� ��û
        MyTCPClient.Instance.SendRequestToServer(sendMessage);

        // ȣ��Ʈ�� ���� ����
        manager.StartHost();
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
