using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public struct VariableInfo
{
    public MonoBehaviour Script;
    public string VariableName;
    public int UpgradeAmount;
    public bool UpgradeBool;
    public float UpgradeMultiplier;
}
public class Upgrade : MonoBehaviour
{
    
    [SerializeField] List<VariableInfo> _variablesToUpdate = new();
    public List<VariableInfo> GetUpgrades()
    {
        return _variablesToUpdate;
    }
}
