using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class EnterGameRoomUI : MonoBehaviour
{
    private Button btn_GameRoom;

    // �����
    //public string serverIPAddress = "127.0.0.1";

    private void Awake()
    {
        btn_GameRoom = GetComponent<Button>();
        btn_GameRoom.onClick.AddListener(EnterGameRoom);
    }

    private void EnterGameRoom()
    {
        var manager = MyGameRoomManager.singleton;

        // ���ӷ� ������ IP �ּҸ� ����
        //manager.networkAddress = serverIPAddress;

        // Ŭ���̾�Ʈ ����
        manager.StartClient();
    }
}
