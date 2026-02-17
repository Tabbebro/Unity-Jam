using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using ScrutableObjects;
public class SandAmounts
{
    public GameObject SandPrefab;
    public int SandCount;
}
public class SandManager : MonoBehaviour
{
    #region Instance
    public static SandManager Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
    #endregion
    SandManagerData _sandManagerData;
    public bool AllSandGoneThrough;
    public int howManyGoneThrough;
    public int howManySands;
    public int MaxSandAmount = 120;
    int _normalSandAmount = 0;
    int _redSandAmount = 0;
    int _blueSandAmount = 0;
    int _goldenSandAmount = 0;
    public event Action OnAllSandWentThrough;
    public event Action CancelAllSandWentThrough;

    public GameObject NormalSandPrefab;
    public GameObject RedSandPrefab;
    public GameObject BlueSandPrefab;
    public GameObject GoldenSandPrefab;
    public Vector2 spawnPoint;
    public bool RedSandUnlocked = false;
    public bool BlueSandUnlocked = false;
    public bool GoldSandUnlocked = false;
    public int RedSandSpawnChance = 0;
    public int BlueSandSpawnChance = 0;
    [ShowProperties] public SO_SandList SandList;
    List<SandAmounts> _sandList = new();
    void Start()
    {
        //SandList.InitializeLootTable();
        SandList = Instantiate(SandList);

        UpgradeManager.Instance.UpgradeHappened += NewUpgrade;


        Hourglass.Instance.OnRotationFinished += OnRotationFinished;

        _sandList.Add(new SandAmounts(){SandPrefab = NormalSandPrefab, SandCount = _sandManagerData.NormalSandAmount});
        _sandList.Add(new SandAmounts(){SandPrefab = RedSandPrefab, SandCount = _sandManagerData.RedSandAmount});
        _sandList.Add(new SandAmounts(){SandPrefab = BlueSandPrefab, SandCount = _sandManagerData.BlueSandAmount});
        _sandList.Add(new SandAmounts(){SandPrefab = GoldenSandPrefab, SandCount = _sandManagerData.GoldenSandAmount});
        StartCoroutine(SpawnLoadedSands(_sandList));
    }
    private void OnDestroy()
    {
        Hourglass.Instance.OnRotationFinished -= OnRotationFinished;
    }
    public SandManagerData GetSandData()
    {
        return _sandManagerData;
    }
    public void SetSandData(SandManagerData data)
    {
        _sandManagerData = data;
    }
    private void NewUpgrade(object name, VariableInfo upgrade)
    {
        if (name.ToString() == "SandCount")
        {
            //SpawnSandGrains(NormalSandPrefab, upgrade.UpgradeAmount);
            SpawnRandomSand(upgrade.UpgradeAmount);
            if (AllSandGoneThrough)
            {
                //print("CancelAllSandWentThrough");
                AllSandGoneThrough = false;
                CancelAllSandWentThrough?.Invoke();
            }

        }
        if (name.ToString() == "RedSandSpawnChance")
        {
            foreach (var item in SandList.ObjectSpawn)
            {
                if (item.Prefab.name.Contains("Red"))
                {
                    item.SpawnChance += upgrade.UpgradeAmount;
                }
            }
        }
        if (name.ToString() == "BlueSandSpawnChance")
        {
            foreach (var item in SandList.ObjectSpawn)
            {
                if (item.Prefab.name.Contains("Blue"))
                {
                    item.SpawnChance += upgrade.UpgradeAmount;
                }
            }
        }
        if (name.ToString() == "RedSandUnlocked")
        {
            if (RedSandUnlocked) return;
            RedSandUnlocked = true;

            foreach (var item in SandList.ObjectSpawn)
            {
                if (item.Prefab.name.Contains("Red"))
                {
                    SpawnSandGrains(item.Prefab, 1);
                    RedSandSpawnChance = 10;
                    item.SpawnChance = 10;
                }
            }
        }
        if (name.ToString() == "BlueSandUnlocked")
        {
            if (BlueSandUnlocked) return;
            BlueSandUnlocked = true;

            foreach (var item in SandList.ObjectSpawn)
            {
                if (item.Prefab.name.Contains("Blue"))
                {
                    SpawnSandGrains(item.Prefab, 1);
                    BlueSandSpawnChance = 10;
                    item.SpawnChance = 10;
                }
            }
        }
        if (name.ToString() == "GoldSandUnlocked")
        {
            if (GoldSandUnlocked) return;
            GoldSandUnlocked = true;

            foreach (var item in SandList.ObjectSpawn)
            {
                if (item.Prefab.name.Contains("Gold"))
                {
                    SpawnSandGrains(item.Prefab, 1);
                    item.SpawnChance = 10;
                }
            }
        }
    }
    IEnumerator SpawnLoadedSands(List<SandAmounts> _sandList)
    {
        for (int i = 0; i < _sandList.Count; i++)
        {
            yield return SpawnSand(_sandList[i].SandPrefab, _sandList[i].SandCount, 0);
        }
    }
    public void SpawnRandomSand(int amount = 1)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject randomSand = SandList.GetRandomObject();
            if (randomSand == null)
            {
                randomSand = NormalSandPrefab;
            }
            StartCoroutine(SpawnSand(randomSand, 1));
        }
    }
    public void SpawnSandGrains(GameObject gameObject, int amount)
    {
        StartCoroutine(SpawnSand(gameObject, amount));
    }
    IEnumerator SpawnSand(GameObject gameObject, int amount, float waitTime = 0.1f)
    {
        if (howManySands >= MaxSandAmount) yield break;

        for (int i = 0; i < amount; i++)
        {
            GameObject sand = Instantiate(gameObject, spawnPoint, quaternion.identity, transform);
            Grain grain = sand.GetComponent<Grain>();

            if (grain.SandType == E_SandType.Normal) _normalSandAmount ++;
            else if (grain.SandType == E_SandType.Red) _redSandAmount ++; 
            else if (grain.SandType == E_SandType.Blue) _blueSandAmount ++; 
            else if (grain.SandType == E_SandType.Golden) _goldenSandAmount ++;

            _sandManagerData.NormalSandAmount = _normalSandAmount;
            _sandManagerData.RedSandAmount = _redSandAmount;
            _sandManagerData.BlueSandAmount = _blueSandAmount;
            _sandManagerData.GoldenSandAmount = _goldenSandAmount;

            howManySands++;
            yield return new WaitForSeconds(waitTime);
        }
    }

    public void SandWentThrough()
    {
        howManyGoneThrough++;
        if (howManyGoneThrough == transform.childCount)
        {
            AllSandGoneThrough = true;
            OnAllSandWentThrough?.Invoke();
        }
    }

    public void OnRotationFinished()
    {
        howManyGoneThrough = 0;
        AllSandGoneThrough = false;
    }
}
public static class ListExtension
{
    /// <summary>
    /// List shuffler
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            T value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }  
    }
}
