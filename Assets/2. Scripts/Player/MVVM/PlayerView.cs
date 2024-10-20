using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    private PlayerViewModel vm;

    private void Awake()
    {
        AddViewModel();
    }

    private void OnDestroy()
    {
        RemoveViewModel();
    }

    private void AddViewModel()
    {
        if(vm == null)
        {
            vm = new PlayerViewModel();
            vm.PropertyChanged += OnPropertyChanged;
            vm.RegisterPlayerHPChanged(this, true);
            vm.RegisterPlayerSkillGaugeChanged(this, true);
        }
    }

    private void RemoveViewModel()
    {
        if(vm!= null)
        {
            vm.RegisterPlayerSkillGaugeChanged(this, false);
            vm.RegisterPlayerHPChanged(this, false);
            vm.PropertyChanged -= OnPropertyChanged;
            vm = null;
        }
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {

    }
}
