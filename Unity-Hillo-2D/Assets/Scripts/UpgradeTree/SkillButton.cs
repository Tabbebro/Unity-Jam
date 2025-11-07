using UnityEngine;
using UnityEngine.UI;
public enum Group
{
    Middle,
    Left,
    Right,
    Down
}
public class SkillButton : MonoBehaviour
{
    Button _button;
    [SerializeField] Group _skillGroup;
    [SerializeField] int _rowLevel = 0;
    [SerializeField] int _currentLevel = 0;
    [SerializeField] int _maxLevel = 5;
    [SerializeField] int _unlockNext = 1;
    UpgradeTree _upgradeTree;
    void Start()
    {
        _upgradeTree = FindAnyObjectByType<UpgradeTree>();
        _button = GetComponent<Button>();
        _button.onClick.AddListener(delegate { OnClicked(); });
        Color color = GetComponent<Image>().color;
        color.a = 0.2f;
        GetComponent<Image>().color = color;
    }
    public void OnClicked()
    {
        if (_currentLevel >= _maxLevel) return;
        _currentLevel++;
        Color color = GetComponent<Image>().color;
        color.a += 0.2f;
        GetComponent<Image>().color = color;
        if (_currentLevel == _unlockNext)
        {
            _upgradeTree.UnlockRow?.Invoke(_skillGroup, _rowLevel);
        }
    }
    public void SetRow(int level)
    {
        _rowLevel = level;
    }
}
