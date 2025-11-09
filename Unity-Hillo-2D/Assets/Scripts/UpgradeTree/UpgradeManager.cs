using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    #region Instance
    public static UpgradeManager Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
    #endregion
    [SerializeField] TextMeshProUGUI _coinAmountText;
    [SerializeField] TextMeshProUGUI _flipAmountText;
    public HourGlassSettingsSO HourGlassSettings;
    [SerializeField] float _nudgeForce = 1;
    public float NudgeForce
    {
        get => _nudgeForce;
        set
        {
            _nudgeForce = value;
            HourGlassSettings.NudgeForce = _nudgeForce;
        }
    }

    public float NudgeCooldown = 1f;
    public float LetThroughPerSecond = 1f;
    public float LetThroughInterval = 1f;
    public float GrainValue = 1f;
    public int SandCount = 1;

    public bool LetThroughUnlocked = false;
    public bool RedSandUnlocked = false;
    public bool BlueSandUnlocked = false;
    public bool AutomaticHourGlassFlipUnlocked = false;

    public float SandResource = 100;
    public float FlipResource = 0;


    // Score Increase Tweening
    Tween _sandCountTween;
    Tween _flipCountTween;

    public event Action<object, object> UpgradeHappened;
    public void RaiseUpgradeHappened(string name, object item)
    {
        UpgradeHappened?.Invoke(name, item);
    }
    public event Action BalanceModified;
    public void RaiseBalanceModified()
    {
        BalanceModified?.Invoke();
    }
    void Start()
    {
        StartSandTween();
        StartFlipTween();

        UpgradeHappened += Testing;
    }

    private void Testing(object obj, object obj2)
    {
        print(obj);
    }

    public bool EnoughResource(float amount) => SandResource >= amount;
    public bool EnoughFlipResource(float amount) => FlipResource >= amount;
    public void ModifySandResource(float amount)
    {
        SandResource += amount;
        StartSandTween();
        RaiseBalanceModified();
    }
    public void ModifyFlipResource(float amount)
    {
        FlipResource += amount;
        StartFlipTween();
        RaiseBalanceModified();
    }

    #region Tweening
    void StartSandTween() {
        KillSandCountTween();

        float startValue = float.Parse(_coinAmountText.text);
        float duration = Mathf.Clamp(Mathf.Abs(SandResource - startValue) * 0.05f, 0.01f, 1f);
        _sandCountTween = DOVirtual.Float(startValue, SandResource, 2f, value => _coinAmountText.text = value.ToString("F0")).SetEase(Ease.OutCubic);
    }
    void KillSandCountTween() {
        if (_sandCountTween != null && _sandCountTween.IsActive()) {
            _sandCountTween.Kill();
            _sandCountTween = null;
        }
    }
    void StartFlipTween() {
        KillFlipCountTween();

        float startValue = float.Parse(_flipAmountText.text);
        float duration = Mathf.Clamp(Mathf.Abs(FlipResource - startValue) * 0.05f, 0.01f, 1f);
        _flipCountTween = DOVirtual.Float(startValue, FlipResource, 2f, value => _flipAmountText.text = value.ToString("F0")).SetEase(Ease.OutCubic);
    }
    void KillFlipCountTween() {
        if (_flipCountTween != null && _flipCountTween.IsActive()) {
            _flipCountTween.Kill();
            _flipCountTween = null;
        }
    }
    #endregion

}
