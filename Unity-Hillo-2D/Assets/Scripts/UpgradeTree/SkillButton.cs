using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
public class SkillButton : MonoBehaviour
{
    [SerializeField] Image _line;
    [SerializeField] Image img; 
    [SerializeField] List<SkillButton> _connections = new();
    [SerializeField] float _lineLength = 1;
    [SerializeField] int _currentLevel = 0;
    [SerializeField] int _maxLevel = 5;
    [SerializeField] int _unlockNext = 1;
    [SerializeField] int _levelUpCost = 5;
    bool _unlocked = false;
    Button _button;
    UpgradeManager _upgradeManager;
    Upgrade _upgrade;
    void Start()
    {
        _line.gameObject.SetActive(false);
        _upgradeManager = UpgradeManager.Instance;

        _button = GetComponent<Button>();
        _upgrade = GetComponent<Upgrade>();
        _button.onClick.AddListener(delegate { OnClicked(); });

        Color color = img.color;
        color.a = 0.2f;
        img.color = color;
    }
    public void OnClicked()
    {
        // Exit if max level
        if (_currentLevel >= _maxLevel) return;

        if (!_upgradeManager.EnoughResource(_levelUpCost)) return;
        _upgradeManager.ModifyResource(-_levelUpCost);

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
                PropertyInfo prop = type.GetProperty(item.VariableName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (field != null)
                {
                    //Debug.LogWarning($"Field '{item.VariableName}' not found on {type.Name}");
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
                    continue;
                }
                else if(prop != null)
                {
                    object currentValue = prop.GetValue(item.Script);
                    if (currentValue is float f)
                    {
                        prop.SetValue(item.Script, f *= item.UpgradeMultiplier);
                    }
                    else if (currentValue is int i)
                    {
                        prop.SetValue(item.Script, i + Mathf.RoundToInt(item.UpgradeAmount));
                    }
                    else if (currentValue is bool b)
                    {
                        prop.SetValue(item.Script, item.UpgradeBool);
                    }
                    else
                    {
                        Debug.LogWarning($"Field '{item.VariableName}' on {type.Name} is not a numeric type.");
                    }
                    continue;
                }
                
            }
        }
        
        // Unlock next connections
        if (_currentLevel == _unlockNext)
        {
            foreach (SkillButton button in _connections)
            {
                button.Unlock();
                button.SetLine(transform.position);
            }
        }
    }
    public void Unlock()
    {
        if (_unlocked) return;
        _unlocked = true;

        gameObject.SetActive(true);

    }
    public void SetLine(Vector3 endPos)
    {
        if (_line.gameObject.activeInHierarchy) return;

        _line.gameObject.SetActive(true);

        Vector3 midPoint = (gameObject.transform.position + endPos) * 0.5f;
        Vector3 direction = endPos - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotation and set position to middle
        _line.transform.rotation = Quaternion.Euler(0, 0, angle);
        _line.transform.position = midPoint;
        
        // Length
        float distance = direction.magnitude * _lineLength;
        _line.rectTransform.sizeDelta = new Vector2(distance, _line.rectTransform.sizeDelta.y);
    }
}
