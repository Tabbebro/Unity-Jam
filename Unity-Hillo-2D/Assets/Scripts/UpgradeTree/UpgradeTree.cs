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
    public Action<Group, int> UnlockRow;
    void Start()
    {
        SetupRow(_leftRows);
        SetupRow(_rightRows);
        SetupRow(_downRows);

        UnlockRow += RowUnlocked;
    }
    void OpenRow(List<UpgradeRow> rowToUnlock, int level)
    {
        if (level >= rowToUnlock.Count) return;

        for (int i = 0; i < rowToUnlock[level].Row.Count; i++)
        {
            rowToUnlock[level].Row[i].gameObject.SetActive(true);
            /* Color color = rowToUnlock[level].Row[i].GetComponent<Image>().color;
            color.a += 0.2f;
            rowToUnlock[level].Row[i].GetComponent<Image>().color = color; */
        }
    }
    private void RowUnlocked(Group group, int row)
    {
        switch (group)
        {
            case Group.Middle:
                OpenRow(_leftRows, row);
                OpenRow(_rightRows, row);
                OpenRow(_downRows, row);
                break;
            case Group.Left:
                OpenRow(_leftRows, row + 1);
                break;
            case Group.Right:
                OpenRow(_rightRows, row + 1);
                break;
            case Group.Down:
                OpenRow(_downRows, row + 1);
                break;
            default:
                break;
        }
    }

    void SetupRow(List<UpgradeRow> rowToSetup)
    {
        foreach (UpgradeRow row in rowToSetup)
        {
            foreach (SkillButton button in row.Row)
            {
                button.SetRow(rowToSetup.IndexOf(row));
                button.gameObject.SetActive(false);
            }
        }
    }
}
