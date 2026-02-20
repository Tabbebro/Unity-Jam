using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_SandList", menuName = "Spawn Tables/SO_SandList")]
public class SO_SandList : ScriptableObject
{
    [System.Serializable]
    public class ObjectToSpawn
    {
        [SerializeField] public GameObject Prefab;
        [Range(0f, 100f)] public float SpawnChance;
    }
    public List<ObjectToSpawn> ObjectSpawn = new();
    public GameObject GetRandomObject()
    {
        //ListExtension.Shuffle(ObjectSpawn);
        
        float _randomValue = UnityEngine.Random.Range(0f, 100f);
        float _cumulativeChance = 0f;
        foreach (ObjectToSpawn encounter in ObjectSpawn)
        {
            _cumulativeChance += encounter.SpawnChance;
            if (_randomValue <= _cumulativeChance)
            {
                return encounter.Prefab;
            }
        }
        return null;
    }
    public void InitializeLootTable()
    {
        float _totalSpawnChance = 0f;
        foreach (ObjectToSpawn encounter in ObjectSpawn)
        {
            _totalSpawnChance += encounter.SpawnChance;
        }

        if (_totalSpawnChance != 100f)
        {
            NormalizeSpawnChances();
        }
    }
    private void NormalizeSpawnChances()
    {
        float _normalizationFactor = 100f / CalculateTotalSpawnChance();
        foreach(ObjectToSpawn encounter in ObjectSpawn)
        {
            encounter.SpawnChance *= _normalizationFactor;
        }
    }
    private float CalculateTotalSpawnChance()
    {
        float _totalSpawnChance = 0f;
        foreach(ObjectToSpawn encounter in ObjectSpawn)
        {
            _totalSpawnChance += encounter.SpawnChance;
        }
        return _totalSpawnChance;
    }
}