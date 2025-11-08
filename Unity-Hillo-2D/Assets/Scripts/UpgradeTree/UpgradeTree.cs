using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UpgradeRow
{
    public List<SkillButton> Row = new();
}
public class UpgradeTree : MonoBehaviour
{
    [SerializeField] List<UpgradeRow> _leftRows = new();
    [SerializeField] List<UpgradeRow> _rightRows = new();
    [SerializeField] List<UpgradeRow> _downRows = new();
    void Start()
    {
        SetupRow(_leftRows);
        SetupRow(_rightRows);
        SetupRow(_downRows);

        InfoBox.Instance.gameObject.SetActive(false);
    }
    void SetupRow(List<UpgradeRow> rowToSetup)
    {
        foreach (UpgradeRow row in rowToSetup)
        {
            foreach (SkillButton button in row.Row)
            {
                button.gameObject.SetActive(false);
            }
        }
    }
}
