using PlayerEventEnum;
using System.ComponentModel;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    private PlayerViewModel vm;

    private void Awake()
    {
        AddViewModel();
    }

    private void OnEnable()
    {
        EventManager<PlayerHUDEvent>.TriggerEvent(PlayerHUDEvent.SpawnPlayer, this);

        //vm.MaxHP = 200;
        //vm.HP = 100;
        vm.RequestPlayerMaxHPChanged(this, 200);
        vm.RequestPlayerHPChanged(this, 100);
    }

    private void OnDestroy()
    {
        EventManager<PlayerHUDEvent>.TriggerEvent(PlayerHUDEvent.DestroyPlayer);

        RemoveViewModel();
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

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch(e.PropertyName)
        {
            case nameof(vm.HP):

                break;
        }
    }
}
