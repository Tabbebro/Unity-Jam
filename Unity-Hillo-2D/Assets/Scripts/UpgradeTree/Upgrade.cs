using System;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    [Serializable]
    public struct VariableInfo
    {
        public MonoBehaviour Script;
        public string VariableName;
        public int UpgradeAmount;
        public bool UpgradeBool;
        public float UpgradeMultiplier;
    }
    [SerializeField] List<VariableInfo> _variablesToUpdate = new();
    public List<VariableInfo> GetUpgrades()
    {
        return _variablesToUpdate;
    }
}
