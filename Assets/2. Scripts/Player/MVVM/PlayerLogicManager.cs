using System;
using System.Collections.Generic;

public class PlayerLogicManager
{
    private static PlayerLogicManager instance = null;

    public static PlayerLogicManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new PlayerLogicManager();
            }

            return instance;
        }
    }

    private Dictionary<PlayerView, Action<float>> _playerHPChangedCallback = new Dictionary<PlayerView, Action<float>>();
    private Dictionary<PlayerView, Action<float>> _playerMaxHPChangedCallback = new Dictionary<PlayerView, Action<float>>();
    private Dictionary<PlayerView, Action<float>> _playerSkillGaugeChangedCallback = new Dictionary<PlayerView, Action<float>>();
    private Dictionary<PlayerView, Action<float>> _playerMaxSkillGaugeChangedCallback = new Dictionary<PlayerView, Action<float>>();

    #region HP
    public void RegisterPlayerHPChangedCallback(PlayerView playerView, Action<float> playerHPChangedCallback, bool isRegister)
    {
        if (isRegister)
        {
            if (!_playerHPChangedCallback.ContainsKey(playerView))
            {
                _playerHPChangedCallback.Add(playerView, playerHPChangedCallback);
            }
            else
            {
                _playerHPChangedCallback[playerView] += playerHPChangedCallback;
            }
        }
        else
        {
            if (_playerHPChangedCallback.ContainsKey(playerView))
            {
                _playerHPChangedCallback[playerView] -= playerHPChangedCallback;
                if (_playerHPChangedCallback[playerView] == null)
                    _playerHPChangedCallback.Remove(playerView);
            }
        }
    }

    public void OnChangedPlayerHP(PlayerView playerView, float hp)
    {
        if (_playerHPChangedCallback.ContainsKey(playerView))
            _playerHPChangedCallback[playerView]?.Invoke(hp);
    }
    #endregion
    #region MaxHP
    public void RegisterPlayerMaxHPChangedCallback(PlayerView playerView, Action<float> playerMaxHPChangedCallback, bool isRegister)
    {
        if (isRegister)
        {
            if (!_playerMaxHPChangedCallback.ContainsKey(playerView))
            {
                _playerMaxHPChangedCallback.Add(playerView, playerMaxHPChangedCallback);
            }
            else
            {
                _playerMaxHPChangedCallback[playerView] += playerMaxHPChangedCallback;
            }
        }
        else
        {
            if (_playerMaxHPChangedCallback.ContainsKey(playerView))
            {
                _playerMaxHPChangedCallback[playerView] -= playerMaxHPChangedCallback;
                if (_playerMaxHPChangedCallback[playerView] == null)
                    _playerMaxHPChangedCallback.Remove(playerView);
            }
        }
    }

    public void OnChangedPlayerMaxHP(PlayerView playerView, float maxHp)
    {
        if (_playerMaxHPChangedCallback.ContainsKey(playerView))
            _playerMaxHPChangedCallback[playerView]?.Invoke(maxHp);
    }
    #endregion
    #region SkillGauge
    public void RegisterPlayerSkillGaugeChangedCallback(PlayerView playerView, Action<float> playerSkillGaugeChangedCallback, bool isRegister)
    {
        if (isRegister)
        {
            if (!_playerSkillGaugeChangedCallback.ContainsKey(playerView))
            {
                _playerSkillGaugeChangedCallback.Add(playerView, playerSkillGaugeChangedCallback);
            }
            else
            {
                _playerSkillGaugeChangedCallback[playerView] += playerSkillGaugeChangedCallback;
            }
        }
        else
        {
            if (_playerSkillGaugeChangedCallback.ContainsKey(playerView))
            {
                _playerSkillGaugeChangedCallback[playerView] -= playerSkillGaugeChangedCallback;
                if (_playerSkillGaugeChangedCallback[playerView] == null)
                    _playerSkillGaugeChangedCallback.Remove(playerView);
            }
        }
    }

    public void OnChangedPlayerSkillGauge(PlayerView playerView, float skillGauge)
    {
        if (_playerSkillGaugeChangedCallback.ContainsKey(playerView))
            _playerSkillGaugeChangedCallback[playerView]?.Invoke(skillGauge);
    }
    #endregion
    #region MaxSkillGauge
    public void RegisterPlayerMaxSkillGaugeChangedCallback(PlayerView playerView, Action<float> playerMaxSkillGaugeChangedCallback, bool isRegister)
    {
        if (isRegister)
        {
            if (!_playerMaxSkillGaugeChangedCallback.ContainsKey(playerView))
            {
                _playerMaxSkillGaugeChangedCallback.Add(playerView, playerMaxSkillGaugeChangedCallback);
            }
            else
            {
                _playerMaxSkillGaugeChangedCallback[playerView] += playerMaxSkillGaugeChangedCallback;
            }
        }
        else
        {
            if (_playerMaxSkillGaugeChangedCallback.ContainsKey(playerView))
            {
                _playerMaxSkillGaugeChangedCallback[playerView] -= playerMaxSkillGaugeChangedCallback;
                if (_playerMaxSkillGaugeChangedCallback[playerView] == null)
                    _playerMaxSkillGaugeChangedCallback.Remove(playerView);
            }
        }
    }

    public void OnChangedPlayerMaxSkillGauge(PlayerView playerView, float maxSkillGauge)
    {
        if (_playerMaxSkillGaugeChangedCallback.ContainsKey(playerView))
            _playerMaxSkillGaugeChangedCallback[playerView]?.Invoke(maxSkillGauge);
    }
    #endregion
}
