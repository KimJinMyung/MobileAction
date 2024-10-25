using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TCP_Enum;

public class JoinGameRoomUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField input_JoinCode;
    [SerializeField] private Button btn_JoinRoom;

    private void Awake()
    {
        btn_JoinRoom.onClick.AddListener(JoinRoom);
    }

    private void JoinRoom()
    {
        EventManager<Tcp_Room_Command>.TriggerEvent(Tcp_Room_Command.enterSelectRoom, input_JoinCode.text);
    }

    private void Update()
    {
        if (string.IsNullOrEmpty(input_JoinCode.text))
        {
            btn_JoinRoom.interactable = false;
        }
        else
        {
            btn_JoinRoom.interactable = true;
        }
    }
}
