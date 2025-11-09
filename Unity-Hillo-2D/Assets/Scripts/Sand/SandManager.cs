using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using ScrutableObjects;
using Unity.VisualScripting;
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
    public bool AllSandGoneThrough;
    public int howManyGoneThrough;
    public int howManySands;
    public int MaxSandAmount = 120;
    public event Action OnAllSandWentThrough;
    public event Action CancelAllSandWentThrough;

    public GameObject NormalSandPrefab;
    public GameObject RedSandPrefab;
    public GameObject BlueSandPrefab;
    public Vector2 spawnPoint;
    public bool RedSandUnlocked = false;
    public bool BlueSandUnlocked = false;
    public int RedSandSpawnChance = 0;
    public int BlueSandSpawnChance = 0;
    [ShowProperties]public SO_SandList SandList;

    void Start()
    {
        //SandList.InitializeLootTable();
        SandList = Instantiate(SandList);

        UpgradeManager.Instance.UpgradeHappened += NewUpgrade;
    
        SpawnSandGrains(NormalSandPrefab, 1);

        Hourglass.Instance.OnRotationFinished += OnRotationFinished;
    }

    private void OnDestroy() {
        Hourglass.Instance.OnRotationFinished -= OnRotationFinished;
    }

    private void NewUpgrade(object name, VariableInfo upgrade)
    {
        if (name.ToString() == "SandCount")
        {
            //SpawnSandGrains(NormalSandPrefab, upgrade.UpgradeAmount);
            SpawnRandomSand(upgrade.UpgradeAmount);
            if (AllSandGoneThrough)
            {
                print("CancelAllSandWentThrough");
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
        if (name.ToString()== "BlueSandUnlocked")
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
            StartCoroutine(SpawnSand(randomSand, amount));
        }
    }
    public void SpawnSandGrains(GameObject gameObject, int amount)
    {
        StartCoroutine(SpawnSand(gameObject, amount));
    }
    IEnumerator SpawnSand(GameObject gameObject, int amount)
    {
        if (howManySands >= MaxSandAmount) yield break;

        for (int i = 0; i < amount; i++)
        {
            GameObject sand = Instantiate(gameObject, spawnPoint, quaternion.identity, transform);
            Grain grain = sand.GetComponent<Grain>();
            howManySands++;
            yield return new WaitForSeconds(0.1f);
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

    public void OnRotationFinished() {
        howManyGoneThrough = 0;
        AllSandGoneThrough = false;
    }
}
