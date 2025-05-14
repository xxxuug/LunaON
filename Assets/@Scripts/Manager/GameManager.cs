using System;
using UnityEngine;

public class PlayerInfo
{
    public float Atk;
    public float Def;
    public int Level;
    public float CurrentExp;
    public float MaxExp;
    public int CurrentHp;
    public int MaxHp;
    public int Gold;
}

public class GameManager : Singleton<GameManager>
{
    #region Unity LifeSteyle Methods
    public GameObject CommonUIPrefab;
    private static GameObject _commonUIInstance;

    private void Awake()
    {
        Initialize();
    }

    protected override void Initialize()
    {
        base.Initialize();

        if (_commonUIInstance == null)
        {
            _commonUIInstance = Instantiate(CommonUIPrefab);
            DontDestroyOnLoad(_commonUIInstance);
        }
    }
    #endregion

    #region Cursor
    public bool IsCursorUnlock = false;

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    #endregion

    #region PlayerInfo
    public event Action OnPlayerInfoChanged;

    private PlayerInfo _playerInfo = new PlayerInfo()
    {
        Atk = 50,
        Def = 0,
        Level = 1,
        CurrentExp = 0,
        MaxExp = 100,
        CurrentHp = 100,
        MaxHp = 100,
        Gold = 0,
    };

    public PlayerInfo PlayerInfo
    {
        get { return _playerInfo; }
        set
        {
            _playerInfo = value;
            OnPlayerInfoChanged?.Invoke();
        }
    }
    #endregion

    public void ChangeHp(int amount)
    {
        _playerInfo.CurrentHp += amount;
        OnPlayerInfoChanged?.Invoke();
    }

    public GameObject LevelUpEffect;
    public PlayerController Player;

    public void AddExp(float amount)
    {
        _playerInfo.CurrentExp += amount;

        if (_playerInfo.CurrentExp >= _playerInfo.MaxExp)
        {
            _playerInfo.CurrentExp -= _playerInfo.MaxExp;
            _playerInfo.Level += 1;
            _playerInfo.MaxExp *= 1.2f;
            _playerInfo.CurrentHp = _playerInfo.MaxHp;
            if (LevelUpEffect != null)
            {
                Instantiate(LevelUpEffect, Player.transform.position, Quaternion.identity, Player.transform);
            }

            Debug.Log($"레벨업! 현재 레벨 : {_playerInfo.Level}");
        }

        OnPlayerInfoChanged?.Invoke();
    }
}
