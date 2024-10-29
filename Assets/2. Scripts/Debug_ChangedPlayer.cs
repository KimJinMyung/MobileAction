using PlayerEventEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Debug_ChangedPlayer : MonoBehaviour
{
    [SerializeField] private Button btn_debug1;
    [SerializeField] private Button btn_debug2;

    private const string debug_Id = "sss";
    private const string debug_Id2 = "aaa";

    private const string debug_Pass = "1345";
    private const string debug_Pass2 = "4567";

    private void Awake()
    {
        btn_debug1.onClick.AddListener(Debub1);
        btn_debug2.onClick.AddListener(Debub2);
    }

    private void Debub1()
    {
        ChangedPlayer(debug_Id, debug_Pass);
    }

    private void Debub2()
    {
        ChangedPlayer(debug_Id2, debug_Pass2);
    }

    private void ChangedPlayer(string ID, string Password)
    {
        EventManager<UserData>.TriggerEvent(UserData.ChangedUser, ID, Password);
    }
}
