using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectManager : Singleton<ObjectManager>
{
    private PlayerController _player;
    public PlayerController Player { get => _player; }

    public HashSet<CactusController> Cactuses { get; set; } = new HashSet<CactusController>();
    public HashSet<MushroomController> Mushrooms { get; set; } = new HashSet<MushroomController>();

    private GameObject _playerResource;
    private GameObject _cactusResource;
    private GameObject _mushroomResource;

    protected override void Initialize()
    {
        base.Initialize();

        if (_player == null)
            _player = FindAnyObjectByType<PlayerController>();

        _playerResource = Resources.Load<GameObject>(Define.PlayerPath);
    }

    public void ResourceAllLoad()
    {
        _playerResource = Resources.Load<GameObject>(Define.PlayerPath);
        _cactusResource = Resources.Load<GameObject>(Define.CactusPath);
        _mushroomResource = Resources.Load<GameObject>(Define.MushroomPath);
    }

    public T Spawn<T>(Vector3 spawnPos) where T : BaseController
    {
        Type type = typeof(T);

        if (type == typeof(PlayerController))
        {
            GameObject obj = Instantiate(_playerResource, spawnPos, Quaternion.identity);
            PlayerController playerController = obj.GetOrAddComponent<PlayerController>();
            _player = playerController;
            return playerController as T;
        }
        else if (type == typeof(CactusController))
        {
            GameObject obj = Instantiate(_cactusResource, spawnPos, Quaternion.identity);
            CactusController cactusController = obj.GetOrAddComponent<CactusController>();
            Cactuses.Add(cactusController);
            return cactusController as T;
        }
        else if (type == typeof(MushroomController))
        {
            GameObject obj = Instantiate(_mushroomResource, spawnPos, Quaternion.identity);
            MushroomController mushroomController = obj.GetOrAddComponent<MushroomController>();
            Mushrooms.Add(mushroomController);
            return mushroomController as T;
        }
        return null;
    }

    public void Despawn<T>(T obj) where T : BaseController
    {
        obj.gameObject.SetActive(false);
    }

    protected override void Clear()
    {
        base.Clear();
        Cactuses.Clear();
        Mushrooms.Clear();
        _player = null;
        Resources.UnloadUnusedAssets();
    }
}
