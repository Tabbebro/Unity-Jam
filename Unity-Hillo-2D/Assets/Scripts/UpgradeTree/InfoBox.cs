using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] TextMeshProUGUI _flipCostText;
    [SerializeField] TextMeshProUGUI _maxedText;
    [SerializeField] Image _coinImage;
    [SerializeField] Image _flipImage;


    public void MoveInfo(string description, int currLevel, int maxLevel, float costSand, float costFlip)
    {
        _descriptionText.text = $"{description}";
        if (currLevel >= maxLevel)
        {
            _maxedText.gameObject.SetActive(true);
        }
        else
        {
            _maxedText.gameObject.SetActive(false);
        }
        _levelText.text = $"{currLevel} / {maxLevel}";
        if (costSand == 0)
        {
            _costText.text = "";
            _coinImage.gameObject.SetActive(false);
        }
        else
        {
            _coinImage.gameObject.SetActive(true);
            _costText.text = SetStringColor(UpgradeManager.Instance.EnoughResource(costSand), costSand);
        }
        if (costFlip == 0)
        {
            _flipCostText.text = "";
            _flipImage.gameObject.SetActive(false);
        }
        else
        {
            
            _flipImage.gameObject.SetActive(true);
            _flipCostText.text = SetStringColor(UpgradeManager.Instance.EnoughFlipResource(costFlip), costFlip);
        }
    }
    string SetStringColor(bool boolean, float cost)
    {
        return boolean ? $"<color=green>{cost}</color>" : $"<color=red>{cost}</color>";
    }
}
