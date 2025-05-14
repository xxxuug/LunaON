using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningPool : Singleton<SpawningPool>
{
    private Coroutine _coRespawnPool;
    WaitForSeconds _spawnInterval = new WaitForSeconds(5f);

    private int _maxSpawnCount = 12; // 최대 스폰 개수

    // 총 슬라임 리스트
    private List<CactusController> _spawnCactus = new List<CactusController>();
    // 죽은 슬라임 리스트
    private List<CactusController> _deadCactus = new List<CactusController>();
    // 총 머쉬룸 리스트
    private List<MushroomController> _spawnMushroom = new List<MushroomController>();
    // 죽은 머쉬룸 리스트
    private List<MushroomController> _deadMushroom = new List<MushroomController>();

    void Start()
    {
        ObjectManager.Instance.ResourceAllLoad();

        for (int i = 0; i < _maxSpawnCount; i++)
        {
            // 선인장
            CactusController cactus = PoolManager.Instance.GetObject<CactusController>(Vector3.zero);

            Vector3 cactusSpawnPos = GetRandomPositionInRage(
                cactus.MinX, cactus.MaxX,
                cactus.MinZ, cactus.MaxZ);

            //cactus.transform.position = spawnPos;
            cactus.SetSpawnPoint(cactusSpawnPos);

            _spawnCactus.Add(cactus);

            // 머쉬룸
            MushroomController mushroom = PoolManager.Instance.GetObject<MushroomController>(Vector3.zero);

            Vector3 mushroomSpawnPos = GetRandomPositionInRage(
                mushroom.MinX, mushroom.MaxX,
                mushroom.MinZ, mushroom.MaxZ);

            mushroom.SetSpawnPoint(mushroomSpawnPos);

            _spawnMushroom.Add(mushroom);
        }
    }

    public void CactusDie(CactusController cactus)
    {
        _deadCactus.Add(cactus);

        if (_coRespawnPool == null)
        {
            _coRespawnPool = StartCoroutine(CoRespawnMonster());
        }
    }

    public void MushroomDie(MushroomController mushroom)
    {
        _deadMushroom.Add(mushroom);

        if (_coRespawnPool == null)
        {
            _coRespawnPool= StartCoroutine(CoRespawnMonster());
        }
    }

    IEnumerator CoRespawnMonster()
    {
        yield return _spawnInterval;

        // 선인장
        foreach (var cactus in _deadCactus)
        {
            Vector3 cactusSpawnPos = GetRandomPositionInRage(
                cactus.MinX, cactus.MaxX,
                cactus.MinZ, cactus.MaxZ);

            cactus.transform.position = cactusSpawnPos;
            cactus.gameObject.SetActive(true);
        }

        // 머쉬룸
        foreach (var mushroom in _deadMushroom)
        {
            Vector3 mushroomSpawnPos = GetRandomPositionInRage(
                mushroom.MinX, mushroom.MaxX,
                mushroom.MinZ, mushroom.MaxZ);

            mushroom.transform.position = mushroomSpawnPos;
            mushroom.gameObject.SetActive(true);
        }

        _deadCactus.Clear();
        _deadMushroom.Clear();
        _coRespawnPool = null;
    }

    private Vector3 GetRandomPositionInRage(float minX, float maxX, float minZ, float maxZ)
    {
        float x = Random.Range(minX, maxX);
        float z = Random.Range(minZ, maxZ);

        Vector3 origin = new Vector3(x, 20f, z);
        RaycastHit hit;

        if (Physics.Raycast(origin, Vector3.down, out hit, 50f, LayerMask.GetMask("Ground")))
        {
            Vector3 spawnPoint = hit.point + Vector3.up * 0.1f;
            return spawnPoint;
        }
        //float y = 5f;

        return new Vector3(x, 20f, z);
    }
}
