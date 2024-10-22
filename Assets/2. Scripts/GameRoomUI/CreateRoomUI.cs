using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using PlayerEventEnum;
using TCP_Enum;

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
        // �� ���� �۾� ó��

        // ȣ��Ʈ�� ���� ����
        manager.StartHost();

        // ȣ��Ʈ�� IP �ּҸ� �߾� ������ ���� ��ũ��Ʈ�� ���
        string hostIP = NetworkManager.singleton.networkAddress;
        string roomID = "261212";

        // �����
        string passward = "1234";

        // TCP ����� ���� DB�� �� ���� ��û
        MyTCPClient.Instance.SendRequestToServer($"{Tcp_Room_Command.createRoom},{roomID},{hostIP},{passward}");  
    }

    private void Debug_PrintRoomList()
    {
        // TCP ����� ���� DB�� �� ���� ��û
        MyTCPClient.Instance.SendRequestToServer($"{Tcp_Room_Command.getRoomList}");
    }

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
