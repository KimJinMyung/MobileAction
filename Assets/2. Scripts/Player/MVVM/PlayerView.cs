using PlayerEventEnum;
using System.ComponentModel;
using UnityEngine;
using Mirror;

public class PlayerView : NetworkBehaviour
{
    private PlayerViewModel vm;

    private void Awake()
    {
        AddViewModel();
    }

    private void OnDestroy()
    {
        EventManager<PlayerHUDEvent>.TriggerEvent(PlayerHUDEvent.DestroyPlayer);

        RemoveViewModel();
    }

    private void OnEnable()
    {
        EventManager<PlayerHUDEvent>.TriggerEvent(PlayerHUDEvent.EnableHUD, true);
    }

    public override void OnStartLocalPlayer()
    {
        SpawnCharacter();
    }

    private void AddViewModel()
    {
        if(vm == null)
        {
            vm = new PlayerViewModel();
            vm.PropertyChanged += OnPropertyChanged;
            vm.RegisterPlayerHPChanged(this, true);
            vm.RegisterPlayerMaxHPChanged(this, true);
            vm.RegisterPlayerSkillGaugeChanged(this, true);
            vm.RegisterPlayerMaxSkillGaugeChanged(this, true);
        }
    }

    private void RemoveViewModel()
    {
        if(vm!= null)
        {
            vm.RegisterPlayerMaxSkillGaugeChanged(this, false);
            vm.RegisterPlayerSkillGaugeChanged(this, false);
            vm.RegisterPlayerMaxHPChanged(this, false);  
            vm.RegisterPlayerHPChanged(this, false);
            vm.PropertyChanged -= OnPropertyChanged;
            vm = null;
        }
    }

    private void SpawnCharacter()
    {
        if (!isLocalPlayer) return;

        EventManager<PlayerHUDEvent>.TriggerEvent(PlayerHUDEvent.SpawnPlayer, this);

        // µð¹ö±ë
        //vm.MaxHP = 200;
        //vm.HP = 100;

        // ÃÊ±â °ª
        vm.RequestPlayerMaxHPChanged(this, 200);
        vm.RequestPlayerHPChanged(this, 100);
        vm.RequestPlayerMaxSkillGaugeChanged(this, 100);
        vm.RequestPlayerSkillGaugeChanged(this, 30);
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch(e.PropertyName)
        {
            case nameof(vm.HP):
                Debug.Log($"µð¹ö±ë : {vm.HP}");
                break;
        }
    }

    private void Update()
    {
        Debug.Log($"{isLocalPlayer} : {isServer} ");

        Debug.Log($"Player View : {vm.HP}");

        if (Input.GetKeyDown(KeyCode.K) && isLocalPlayer)
        {
            float hp = vm.HP;
            hp -= 10f;

            vm.RequestPlayerHPChanged(this, hp);
        }
    }
}
