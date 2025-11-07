using System.Collections.Generic;
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
    Image img; 
    void Start()
    {
        _upgradeTree = FindAnyObjectByType<UpgradeTree>();

        _button = GetComponent<Button>();
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
