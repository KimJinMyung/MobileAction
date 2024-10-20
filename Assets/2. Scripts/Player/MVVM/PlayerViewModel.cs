using System.ComponentModel;

public class PlayerViewModel
{
    private float hp;
    public float HP
    {
        get { return hp; }
        set
        {
            if(hp == value) return;
            hp = value;
            OnPropertyChanged(nameof(HP));
        }
    }

    private float skillGauge;
    public float SkillGauge
    {
        get => skillGauge;
        set
        {
            if(skillGauge == value) return;
            skillGauge = value;
            OnPropertyChanged(nameof(SkillGauge));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
