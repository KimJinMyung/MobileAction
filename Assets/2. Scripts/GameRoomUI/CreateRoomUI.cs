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
        // �� ���� �۾� ó��

        // ȣ��Ʈ�� ���� ����
        manager.StartHost();

        // ȣ��Ʈ�� IP �ּҸ� �߾� ������ ���� ��ũ��Ʈ�� ���
        string hostIP = NetworkManager.singleton.networkAddress;
        string roomID = NetworkManager.singleton.GetInstanceID().ToString();

        // �����
        string passward = "0000";

        EventManager<DB_Event>.TriggerEvent(DB_Event.CreateRoom, hostIP, roomID, passward);


        // �����
        Debug.Log(hostIP);
    }
}
