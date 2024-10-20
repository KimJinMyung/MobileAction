public static class PlayerHUDViewModelExtenstion
{
    #region HP
    public static void RegisterPlayerHPChanged(this PlayerHUDViewModel vm, PlayerView view, bool isRegister)
    {
        PlayerLogicManager.Instance.RegisterPlayerHPChangedCallback(view, vm.OnResponseChangedPlayerHP, isRegister);
    }

    public static void RequestPlayerHPChanged(this PlayerHUDViewModel vm, PlayerView view, float hp)
    {
        PlayerLogicManager.Instance.OnChangedPlayerHP(view, hp);
    }

    public static void OnResponseChangedPlayerHP(this PlayerHUDViewModel vm, float hp)
    {
        vm.HP = hp;
    }
    #endregion
    #region MaxHP
    public static void RegisterPlayerMaxHPChanged(this PlayerHUDViewModel vm, PlayerView view, bool isRegister)
    {
        PlayerLogicManager.Instance.RegisterPlayerMaxHPChangedCallback(view, vm.OnResponseChangedPlayerMaxHP, isRegister);
    }

    public static void RequestPlayerMaxHPChanged(this PlayerHUDViewModel vm, PlayerView view, float maxHp)
    {
        PlayerLogicManager.Instance.OnChangedPlayerMaxHP(view, maxHp);
    }

    public static void OnResponseChangedPlayerMaxHP(this PlayerHUDViewModel vm, float maxHp)
    {
        vm.MaxHP = maxHp;
    }
    #endregion
    #region SkillGauge
    public static void RegisterPlayerSkillGaugeChanged(this PlayerHUDViewModel vm, PlayerView view, bool isRegister)
    {
        PlayerLogicManager.Instance.RegisterPlayerSkillGaugeChangedCallback(view, vm.OnResponseChangedPlayerSkillGauge, isRegister);
    }

    public static void RequestPlayerSkillGaugeChanged(this PlayerHUDViewModel vm, PlayerView view, float skillGauge)
    {
        PlayerLogicManager.Instance.OnChangedPlayerSkillGauge(view, skillGauge);
    }

    public static void OnResponseChangedPlayerSkillGauge(this PlayerHUDViewModel vm, float skillGauge)
    {
        vm.SkillGauge = skillGauge;
    }
    #endregion
    #region MaxSkillGauge
    public static void RegisterPlayerMaxSkillGaugeChanged(this PlayerHUDViewModel vm, PlayerView view, bool isRegister)
    {
        PlayerLogicManager.Instance.RegisterPlayerMaxSkillGaugeChangedCallback(view, vm.OnResponseChangedPlayerMaxSkillGauge, isRegister);
    }

    public static void RequestPlayerMaxSkillGaugeChanged(this PlayerHUDViewModel vm, PlayerView view, float maxSkillGauge)
    {
        PlayerLogicManager.Instance.OnChangedPlayerMaxSkillGauge(view, maxSkillGauge);
    }

    public static void OnResponseChangedPlayerMaxSkillGauge(this PlayerHUDViewModel vm, float maxSkillGauge)
    {
        vm.MaxSkillGauge = maxSkillGauge;
    }
    #endregion
}
