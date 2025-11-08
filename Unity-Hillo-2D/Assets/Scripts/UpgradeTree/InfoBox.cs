using TMPro;
using UnityEngine;

public class InfoBox : MonoBehaviour
{
    #region Instance
    public static InfoBox Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
    #endregion

    [SerializeField] TextMeshProUGUI _descriptionText;
    [SerializeField] TextMeshProUGUI _levelText;
    [SerializeField] TextMeshProUGUI _costText;


    public void MoveInfo(string description, int currLevel, int maxLevel, int cost)
    {
        _descriptionText.text = $"{description}";
        _levelText.text = $"{currLevel} / {maxLevel}";
        _costText.text = SetStringColor(UpgradeManager.Instance.EnoughResource(cost), cost);
    }
    string SetStringColor(bool boolean, int cost)
    {
        return boolean ? $"<color=green>{cost}</color>" : $"<color=red>{cost}</color>";
    }
}
