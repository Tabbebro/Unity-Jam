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
    public int howManySands;
    public event Action OnAllSandWentThrough;
    public event Action CancelAllSandWentThrough;

    public GameObject NormalSandPrefab;
    public GameObject RedSandPrefab;
    public GameObject BlueSandPrefab;
    public Vector2 spawnPoint;
    

    void Start()
    {
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
            SpawnSandGrains(NormalSandPrefab, upgrade.UpgradeAmount);

            if (AllSandGoneThrough) 
            {
                print("CancelAllSandWentThrough");
                AllSandGoneThrough = false;
                CancelAllSandWentThrough?.Invoke();
            }
            
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
