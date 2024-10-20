using System.ComponentModel;

public class PlayerHUDViewModel
{
    private float hp;
    public float HP
    {
        get { return hp; }
        set
        {
            if (hp == value) return;
            hp = value;
            OnPropertyChanged(nameof(HP));
        }
    }

    private float maxHp;
    public float MaxHP
    {
        get { return maxHp; }
        set
        {
            if (maxHp == value) return;
            maxHp = value;
            OnPropertyChanged(nameof(MaxHP));
        }
    }

    private float skillGauge;
    public float SkillGauge
    {
        get => skillGauge;
        set
        {
            if (skillGauge == value) return;
            skillGauge = value;
            OnPropertyChanged(nameof(SkillGauge));
        }
    }

    private float maxSkillGauge;
    public float MaxSkillGauge
    {
        get => maxSkillGauge;
        set
        {
            if (maxSkillGauge == value) return;
            maxSkillGauge = value;
            OnPropertyChanged(nameof(MaxSkillGauge));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
