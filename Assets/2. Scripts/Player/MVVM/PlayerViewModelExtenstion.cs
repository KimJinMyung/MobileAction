public static class PlayerViewModelExtenstion
{
    #region HP
    public static void RegisterPlayerHPChanged(this PlayerViewModel vm, PlayerView view, bool isRegister)
    {
        PlayerLogicManager.Instance.RegisterPlayerHPChangedCallback(view, vm.OnResponseChangedPlayerHP, isRegister);
    }

    public static void RequestPlayerHPChanged(this PlayerViewModel vm, PlayerView view, float hp)
    {
        PlayerLogicManager.Instance.OnChangedPlayerHP(view, hp);
    }

    public static void OnResponseChangedPlayerHP(this PlayerViewModel vm, float hp)
    {
        vm.HP = hp;
    }
    #endregion
    #region SkillGauge
    public static void RegisterPlayerSkillGaugeChanged(this PlayerViewModel vm, PlayerView view, bool isRegister)
    {
        PlayerLogicManager.Instance.RegisterPlayerSkillGaugeChangedCallback(view, vm.OnResponseChangedPlayerSkillGauge, isRegister);
    }

    public static void RequestPlayerSkillGaugeChanged(this PlayerViewModel vm, PlayerView view, float skillGauge)
    {
        PlayerLogicManager.Instance.OnChangedPlayerSkillGauge(view, skillGauge);
    }

    public static void OnResponseChangedPlayerSkillGauge(this PlayerViewModel vm, float skillGauge)
    {
        vm.SkillGauge = skillGauge;
    }
    #endregion
}
