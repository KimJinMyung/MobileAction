using PlayerEventEnum;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoostButton : MonoBehaviour
{
    private Button btn_Boost;

    private void Awake()
    {
        btn_Boost = GetComponent<Button>();
        btn_Boost.onClick.AddListener(Booster);
    }

    private void Booster()
    {
        EventManager<PlayerController>.TriggerEvent(PlayerController.Boost);
    }

}
