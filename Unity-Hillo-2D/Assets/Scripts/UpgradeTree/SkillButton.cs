using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SkillButton : MonoBehaviour
{
    [Space]
    [Header("Stats")]
    [SerializeField] Sprite _icon;
    [SerializeField] string _description;
    [SerializeField] int _currentLevel = 0;
    [SerializeField] int _maxLevel = 5;
    [SerializeField] public int UnlockLevel = 1;
    [SerializeField] float _levelUpResourceCost = 5;
    [SerializeField] float _levelUpFlipCost = 5;
    [SerializeField] float _levelUpMultiplier = 1.2f;


    [Header("References")]
    [SerializeField] Image _line;
    [SerializeField] Image _middleIcon;
    [SerializeField] Image img;
    [SerializeField] Image _hoverImg;
    [SerializeField] Image _fadeImg;
    [SerializeField] TextMeshProUGUI _maxText;
    [SerializeField] List<SkillButton> _connections = new();

    [Space]
    [Header("Hover")]
    [SerializeField] float _hoverDuration = 0.1f;
    [SerializeField] Vector3 _endScale = new();
    Coroutine _hoverCoroutine = null;

    [Space]
    [Header("Settings")]
    [SerializeField] float _lineLength = 1;
    [SerializeField] float _offsetY = 200;
    [SerializeField] float _offsetX = 200;
    [SerializeField] float _yDeadZone = 150;
    [SerializeField] float _xDeadZone = 150;
    [SerializeField] Color _fadedBorderColor;
    [SerializeField] Color _fullBorderColor;

    [Space]
    [Header("Animation settings")]
    [SerializeField] Vector2 _scaleIncrease = new Vector2(1.2f, 1.2f);
    [SerializeField] float _animationDuration = 0.2f;
    Coroutine _scaleCoroutine = null;

    [Space]
    [Header("Audio")]
    [SerializeField] public List<AudioClip> _audioClips = new();

    bool _unlocked = false;
    bool _hovering = false;
    Button _button;
    UpgradeManager _upgradeManager;
    UpgradeTree _upgradeTree;
    RectTransform _borders;
    public AudioSource _audio;
    Upgrade _upgrade;
    RectTransform _rect;
    Vector3 _startScale;
    Queue<Action> _upgradeActionQueue = new();
    Action _currentUpgradeAction = null;
    void Start()
    {
        if (_icon != null)
        {
            _middleIcon.sprite = _icon;
        }
        if (_currentLevel <= 0)
        {
            _hoverImg.color = _fadedBorderColor;
        }

        _startScale = transform.localScale;

        _upgradeManager = UpgradeManager.Instance;
        
        _upgradeManager.BalanceModified += CheckBalance;
        //_line.gameObject.SetActive(false);

        _button = GetComponent<Button>();
        _rect = GetComponent<RectTransform>();
        //_audio = GetComponent<AudioSource>();
        _upgradeTree = FindAnyObjectByType<UpgradeTree>();
        _borders = _upgradeTree.GetComponent<RectTransform>();
        _upgrade = GetComponent<Upgrade>();
        _button.onClick.AddListener(delegate { OnClicked(); });
        Hourglass.Instance.OnRotationFinished += CheckActionQueue;
        /* Color color = img.color;
        color.a = 0.2f;
        img.color = color; */
        CheckBalance();
    }

    private void CheckActionQueue()
    {
        if (_upgradeActionQueue.Count <= 0) return;
        int actionAmount = _upgradeActionQueue.Count;

        for (int i = 0; i < actionAmount; i++)
        {
            _currentUpgradeAction = _upgradeActionQueue.Dequeue();
            _currentUpgradeAction?.Invoke();
        }
        
    }

    public void PointerEnter()
    {
        if (Zoom.Instance.IsDragging) return;

        _audio.clip = _audioClips[4];
        _audio.Play();
        _hovering = true;
        if (_hoverCoroutine != null)
        {
            StopCoroutine(_hoverCoroutine);
            _hoverCoroutine = null;
        }
        _hoverCoroutine = StartCoroutine(Scale(_endScale, _hoverImg.gameObject));
        _hoverImg.color = _fullBorderColor;

        // Apply offsets in UI space
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, transform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _borders.parent as RectTransform,
            screenPoint,
            null,
            out Vector2 localPoint
        );
        localPoint += new Vector2(0, _offsetY);

        //Vector2 bounds = _borders.rect.size;
        if (localPoint.y > 400) // Top
        {
            localPoint += new Vector2(0, -_offsetY * 2);
        }
        if (localPoint.x < -300) // Left
        {
            localPoint += new Vector2(_offsetX, 0);
        }
        if (localPoint.x > 300) // Right
        {
            localPoint += new Vector2(-_offsetX, 0);
        }

        // Assign the local position
        InfoBox.Instance.GetComponent<RectTransform>().anchoredPosition = localPoint;

        InfoBox.Instance.gameObject.SetActive(true);
        InfoBox.Instance.MoveInfo(_description, _currentLevel, _maxLevel, _levelUpResourceCost, _levelUpFlipCost);
    }
    public void PointerExit()
    {
        _hovering = false;
        if (_hoverCoroutine != null)
        {
            StopCoroutine(_hoverCoroutine);
            _hoverCoroutine = null;
        }
        _hoverCoroutine = StartCoroutine(Scale(new Vector3(1, 1, 1), _hoverImg.gameObject));
        if (_currentLevel >= 1)
        {
            _hoverImg.color = _fullBorderColor;
        }
        else
        {
            _hoverImg.color = _fadedBorderColor;
        }
        InfoBox.Instance.gameObject.SetActive(false);
    }
    public void OnClicked()
    {


        // Exit if max level
        if (_currentLevel >= _maxLevel)
        {
            _audio.clip = _audioClips[1];
            _audio.Play();
            return;
        }
        if (!_upgradeManager.EnoughResource(_levelUpResourceCost) || !_upgradeManager.EnoughFlipResource(_levelUpFlipCost))
        {
            _audio.clip = _audioClips[3];
            _audio.Play();
            return;
        }
        if (Hourglass.Instance.IsRotating)
        {
            Upgrade(false);
            if (_upgrade != null)
            {
                foreach (var item in _upgrade.GetUpgrades())
                {
                    if (item.Script == null || string.IsNullOrEmpty(item.VariableName))
                        continue;

                    var type = item.Script.GetType();
                    var field = type.GetField(item.VariableName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    //PropertyInfo prop = type.GetProperty(item.VariableName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    _upgradeActionQueue.Enqueue(() => InvokeUgrade(field, item));
                }
            }
            return;
        }

        Upgrade();
    }
    void Upgrade(bool invokeUpgrade = true)
    {
        _upgradeManager.ModifySandResource(-_levelUpResourceCost);
        _upgradeManager.ModifyFlipResource(-_levelUpFlipCost);

        if (_scaleCoroutine != null)
        {
            StopCoroutine(_scaleCoroutine);
            _scaleCoroutine = null;
        }
        _scaleCoroutine = StartCoroutine(TransitionScale(_scaleIncrease, gameObject));

        _levelUpResourceCost *= _levelUpMultiplier;
        _levelUpResourceCost = Mathf.Round(_levelUpResourceCost * 10.0f) * 0.1f;
        _levelUpFlipCost *= _levelUpMultiplier;
        _levelUpFlipCost = Mathf.Round(_levelUpFlipCost * 10.0f) * 0.1f;

        _currentLevel++;
        if (_currentLevel >= 1)
        {
            _hoverImg.color = _fullBorderColor;
        }
        if (_currentLevel >= _maxLevel)
        {
            _maxText.gameObject.SetActive(true);
            _fadeImg.gameObject.SetActive(false);
            _audio.clip = _audioClips[2];
            _audio.Play();
            _levelUpResourceCost = 0;
            _levelUpFlipCost = 0;
        }
        else
        {
            _audio.clip = _audioClips[0];
            _audio.Play();
        }
        InfoBox.Instance.MoveInfo(_description, _currentLevel, _maxLevel, _levelUpResourceCost, _levelUpFlipCost);

        if (_upgrade != null && invokeUpgrade)
        {
            foreach (var item in _upgrade.GetUpgrades())
            {
                if (item.Script == null || string.IsNullOrEmpty(item.VariableName))
                    continue;

                var type = item.Script.GetType();
                var field = type.GetField(item.VariableName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                //PropertyInfo prop = type.GetProperty(item.VariableName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                //if (invokeUpgrade)
                InvokeUgrade(field, item);
            }
        }
        foreach (SkillButton button in _connections)
        {
            if (button.UnlockLevel == _currentLevel)
            {
                
                button.Unlock(transform.position);
                //button.SetLine(transform.position);
                button.CheckBalance();
                button._audio.clip = _audioClips[5];
                button._audio.Play();
            }
        }
    }
    void InvokeUgrade(FieldInfo field, VariableInfo item)
    {
        UpgradeManager.Instance.RaiseUpgradeHappened(item.VariableName, item);
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
                //Debug.LogWarning($"Field '{item.VariableName}' on {type.Name} is not a numeric type.");
            }
            //continue;
        }
    }
    public void CheckBalance()
    {
        if (!UpgradeManager.Instance.EnoughResource(_levelUpResourceCost) || !UpgradeManager.Instance.EnoughFlipResource(_levelUpFlipCost))
        {
            _fadeImg.gameObject.SetActive(true);
        }
        else
        {
            _fadeImg.gameObject.SetActive(false);
        }

        if (InfoBox.Instance.gameObject.activeInHierarchy && _hovering)
        {
            InfoBox.Instance.MoveInfo(_description, _currentLevel, _maxLevel, _levelUpResourceCost, _levelUpFlipCost);
        }
    }
    public void Unlock(Vector3 posForLine)
    {
        if (_unlocked) return;
        _unlocked = true;

        gameObject.SetActive(true);
        SetLine(posForLine);
    }
    public bool LineActive = false;
    public void SetLine(Vector3 endPos)
    {
        //if (_line.gameObject.activeInHierarchy) return;
        if (LineActive) return;
        LineActive = true;

        _line.gameObject.SetActive(true);
        transform.GetChild(0).gameObject.SetActive(true);

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
    IEnumerator Scale(Vector2 endScale, GameObject obj)
    {
        float timePassed = 0f;
        Vector2 startScale = obj.transform.localScale;

        while (timePassed < _animationDuration)
        {
            timePassed += Time.deltaTime;
            float t = timePassed / _animationDuration;

            obj.transform.localScale = Vector2.Lerp(startScale, endScale, t);

            yield return null;
        }
        obj.transform.localScale = endScale;
        _hoverCoroutine = null;
    }
    IEnumerator TransitionScale(Vector2 endScale, GameObject obj)
    {
        float timePassed = 0f;
        Vector2 startScale = obj.transform.localScale;

        while (timePassed < _animationDuration)
        {
            timePassed += Time.deltaTime;
            float t = timePassed / _animationDuration;

            obj.transform.localScale = Vector2.Lerp(startScale, endScale, t);

            yield return null;
        }
        timePassed = 0f;
        while (timePassed < _animationDuration)
        {
            timePassed += Time.deltaTime;
            float t = timePassed / _animationDuration;

            obj.transform.localScale = Vector2.Lerp(endScale, _startScale, t);

            yield return null;
        }
        obj.transform.localScale = _startScale;
        _scaleCoroutine = null;
    }
}
