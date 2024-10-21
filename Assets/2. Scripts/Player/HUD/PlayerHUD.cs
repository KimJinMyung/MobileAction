using PlayerEventEnum;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private Image _hpBar;
    [SerializeField] private Image _staminaBar;

    private PlayerHUDViewModel vm;

    private PlayerView view;
    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();

        AddEvent();
    }

    private void Start()
    {
        canvas.enabled = false;
    }

    private void OnEnable()
    {
        EventManager<Init>.TriggerEvent(Init.SpawnPlayCharacter);
    }

    private void OnDestroy()
    {
        RemoveEvent();
    }

    private void AddEvent()
    {
        EventManager<PlayerHUDEvent>.Binding<PlayerView>(true, PlayerHUDEvent.SpawnPlayer, SpawnPlayer);
        EventManager<PlayerHUDEvent>.Binding(true, PlayerHUDEvent.DestroyPlayer, DestroyPlayer);
        EventManager<PlayerHUDEvent>.Binding<bool>(true, PlayerHUDEvent.EnableHUD, EnablePlayerHUD);
    }

    private void RemoveEvent()
    {
        EventManager<PlayerHUDEvent>.Binding<PlayerView>(false, PlayerHUDEvent.SpawnPlayer, SpawnPlayer);
        EventManager<PlayerHUDEvent>.Binding(false, PlayerHUDEvent.DestroyPlayer, DestroyPlayer);
        EventManager<PlayerHUDEvent>.Binding<bool>(false, PlayerHUDEvent.EnableHUD, EnablePlayerHUD);
    }

    private void EnablePlayerHUD(bool isEnable)
    {
        canvas.enabled = isEnable;
    }

    private void SpawnPlayer(PlayerView view)
    {        
        this.view = view;

        AddViewMode();
    }

    private void DestroyPlayer()
    {
        EnablePlayerHUD(false);

        ReMoveViewMode();

        this.view = null;
    }

    private void AddViewMode()
    {
        if (vm == null)
        {
            vm = new PlayerHUDViewModel();
            vm.PropertyChanged += OnPropertyChanged;
            vm.RegisterPlayerHPChanged(view, true);
            vm.RegisterPlayerMaxHPChanged(view, true);
            vm.RegisterPlayerSkillGaugeChanged(view, true);
            vm.RegisterPlayerMaxSkillGaugeChanged(view, true);
        }
    }

    private void ReMoveViewMode()
    {
        if (vm != null)
        {
            vm.RegisterPlayerMaxSkillGaugeChanged(view, false);
            vm.RegisterPlayerSkillGaugeChanged(view, false);
            vm.RegisterPlayerMaxHPChanged(view, false);
            vm.RegisterPlayerHPChanged(view, false);
            vm.PropertyChanged -= OnPropertyChanged;
            vm = null;
        }
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(vm.HP):
                _hpBar.fillAmount = Mathf.Clamp((float)vm.HP / vm.MaxHP, 0f, vm.MaxHP);
                break;
            case nameof(vm.MaxSkillGauge):
                //Debug.Log(vm.MaxStamina);
                break;
            case nameof(vm.SkillGauge):
                _staminaBar.fillAmount = Mathf.Clamp((float)vm.SkillGauge / vm.MaxSkillGauge, 0f, vm.MaxSkillGauge);
                break;
        }
    }

    private void Update()
    {
        if (canvas.enabled)
        {
            //Debug.Log($"HUD : {vm.HP}");
        }
    }
}
