using System;
using System.Collections;
using NUnit.Framework.Constraints;
using Unity.Mathematics;
using UnityEngine;

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
    public event Action OnAllSandWentThrough;

    public GameObject NormalSandPrefab;
    public GameObject RedSandPrefab;
    public GameObject BlueSandPrefab;
    public Vector2 spawnPoint;
    

    void Start()
    {
        UpgradeManager.Instance.UpgradeHappened += NewUpgrade;
    
        SpawnSandGrains(NormalSandPrefab, 1);
    }

    private void NewUpgrade(object name, object item)
    {
        print(name);
        if (name.ToString() == "SandCount")
        {
            print("Sandcount upgrade came through");
            SpawnSandGrains(NormalSandPrefab, 1);
        }
    }

    public void SpawnSandGrains(GameObject gameObject, int amount)
    {
        StartCoroutine(SpawnSand(gameObject, amount));
    }
    IEnumerator SpawnSand(GameObject gameObject, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject sand = Instantiate(NormalSandPrefab, spawnPoint, quaternion.identity, transform);
            Grain grain = sand.GetComponent<Grain>();
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
}
