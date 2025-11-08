using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SkillButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _costText;
    [SerializeField] TextMeshProUGUI _levelText;
    [SerializeField] TextMeshProUGUI _mainText;
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
    UpgradeTree _upgradeTree;
    RectTransform _borders;
    Upgrade _upgrade;
    void Start()
    {
        _upgradeManager = UpgradeManager.Instance;

        _levelUpCost = GetLevelUpCost(_upgradeManager.TotalLevel);
        /* _costText.text = _levelUpCost.ToString();
        _levelText.text = _currentLevel.ToString(); */

        _line.gameObject.SetActive(false);

        _button = GetComponent<Button>();
        _upgradeTree = FindAnyObjectByType<UpgradeTree>();
        _borders = _upgradeTree.GetComponent<RectTransform>();
        _upgrade = GetComponent<Upgrade>();
        _button.onClick.AddListener(delegate { OnClicked(); });

        Color color = img.color;
        color.a = 0.2f;
        img.color = color;
    }
    public int CostIncrease(int level)
    {
        double points = 0;

        for (int lvl = 1; lvl < level; lvl++)
        {
            points += Math.Floor(lvl + 300.0 * Math.Pow(2.0, lvl / 7.0));
        }

        return (int)Math.Floor(points / 4);
    }
    public int GetLevelUpCost(int currentLevel)
    {
        int totalCurrent = CostIncrease(currentLevel);
        int totalNext = CostIncrease(currentLevel + 1);
        return totalNext - totalCurrent;
    }
    public void PointerEnter()
    {
        Vector2 myPos = transform.position;
        myPos += new Vector2(0, 200);
        Vector2 halfSize = InfoBox.Instance.transform.GetComponent<RectTransform>().rect.size;
        Vector2 bounds = _borders.rect.size;
        
        if (myPos.y > bounds.y - 80)
        {
            myPos += new Vector2(0, -400);
        }
        if (myPos.x < bounds.x + 150)
        {
            myPos += new Vector2(200, 0);
        }
        if (myPos.x > bounds.x * 2 - 150)
        {
            myPos += new Vector2(-200, 0);
        }

        float clampedX = Mathf.Clamp(myPos.x, -bounds.x + myPos.x, bounds.x - myPos.x);
        float clampedY = Mathf.Clamp(myPos.y, -bounds.y + myPos.y, bounds.y - myPos.y);

        //new Vector2(clampedX, clampedY)
        InfoBox.Instance.transform.position = myPos;
        InfoBox.Instance.gameObject.SetActive(true);
        InfoBox.Instance.MoveInfo("Juu level up", _currentLevel, _maxLevel, _levelUpCost);
    }
    public void PointerExit()
    {
        InfoBox.Instance.gameObject.SetActive(false);
    }
    public void OnClicked()
    {
        // Exit if max level
        if (_currentLevel >= _maxLevel) return;

        if (!_upgradeManager.EnoughResource(_levelUpCost)) return;
        _upgradeManager.ModifyResource(-_levelUpCost);

        _currentLevel++;
        _upgradeManager.TotalLevel++;
        //_levelText.text = _currentLevel.ToString();
        _levelUpCost = GetLevelUpCost(_upgradeManager.TotalLevel);
        InfoBox.Instance.MoveInfo("Juu level up", _currentLevel, _maxLevel, _levelUpCost);
        //_costText.text = _levelUpCost.ToString();

        // Increase level and albedo
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
        _line.transform.GetChild(0).rotation = Quaternion.Euler(0, 0, angle);
        _line.transform.position = midPoint;
        _line.transform.GetChild(0).position = midPoint;
        
        // Length
        float distance = direction.magnitude * _lineLength;
        _line.rectTransform.sizeDelta = new Vector2(distance, _line.rectTransform.sizeDelta.y);
        _line.transform.GetChild(0).GetComponent<Image>().rectTransform.sizeDelta = new Vector2(distance, _line.rectTransform.sizeDelta.y);
    }
}
