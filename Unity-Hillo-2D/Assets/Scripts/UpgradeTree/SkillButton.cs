using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
public class SkillButton : MonoBehaviour
{
    Button _button;
    [SerializeField] List<SkillButton> _connections = new();
    [SerializeField] int _rowLevel = 0;
    [SerializeField] int _rowIndex = 0;
    [SerializeField] int _currentLevel = 0;
    [SerializeField] int _maxLevel = 5;
    [SerializeField] int _unlockNext = 1;
    UpgradeTree _upgradeTree;
    Upgrade _upgrade;
    Image img; 
    void Start()
    {
        _upgradeTree = FindAnyObjectByType<UpgradeTree>();

        _button = GetComponent<Button>();
        _upgrade = GetComponent<Upgrade>();
        _button.onClick.AddListener(delegate { OnClicked(); });
        img = GetComponent<Image>();

        Color color = img.color;
        color.a = 0.2f;
        img.color = color;
    }
    public void OnClicked()
    {
        // Exit if max level
        if (_currentLevel >= _maxLevel) return;

        // Increase level and albedo
        _currentLevel++;
        Color color = img.color;
        color.a += 0.2f;
        img.color = color;

        if (_upgrade != null)
        {
            foreach (var item in _upgrade.GetUpgrades())
            {
                if (item.Script == null || string.IsNullOrEmpty(item.VariableName))
                    continue;

                var type = item.Script.GetType();
                var field = type.GetField(item.VariableName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (field == null)
                {
                    Debug.LogWarning($"Field '{item.VariableName}' not found on {type.Name}");
                    continue;
                }
                object currentValue = field.GetValue(item.Script);
                if (currentValue is float f)
                {
                    field.SetValue(item.Script, f *= item.UpgradeMultiplier);
                }
                else if (currentValue is int i)
                {
                    field.SetValue(item.Script, i + Mathf.RoundToInt(item.UpgradeAmount));
                }
                else if (currentValue is bool b)
                {
                    field.SetValue(item.Script, item.UpgradeBool);
                }
                else
                {
                    Debug.LogWarning($"Field '{item.VariableName}' on {type.Name} is not a numeric type.");
                }
            }
        }
        
        // Unlock next connections
        if (_currentLevel == _unlockNext)
        {
            foreach (SkillButton button in _connections)
            {
                button.Unlock();
            }
        }
    }
    public void Unlock()
    {
        gameObject.SetActive(true);
    }
    public void SetRow(int level, int index)
    {
        _rowLevel = level;
        _rowIndex = index;
    }
}
