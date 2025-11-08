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

    public bool RedSandUnlocked = false;
    public bool BlueSandUnlocked = false;
    public bool AutomaticHourGlassFlipUnlocked = false;

    public float SandResource = 100;
    public float FlipResource = 0;

    [Header("Score increaser")]
    bool _isCountingSand = false;
    bool _isCountingFlip = false;
    Coroutine _countingSand;
    Coroutine _countingFlip;
    [SerializeField] int _addAmountPerTick = 200;
    private int _sandResourceToAdd;
    private int _flipResourceToAdd;
    public event Action<object> UpgradeHappened;
    public void RaiseUpgradeHappened(string obj)
    {
        UpgradeHappened?.Invoke(obj);
    }
    void Start()
    {
        _coinAmountText.text = SandResource.ToString();
        _flipAmountText.text = FlipResource.ToString();

        UpgradeHappened += Testing;
    }

    private void Testing(object obj)
    {
        print(obj);
    }

    void Update()
    {
        if (!_isCountingSand)
        {
            _countingSand = StartCoroutine(CountSand());
        }
        if (!_isCountingFlip)
        {
            _countingFlip = StartCoroutine(CountFlip());
        }
    }
    public bool EnoughResource(float amount) => SandResource >= amount;
    public bool EnoughFlipResource(float amount) => FlipResource >= amount;
    public void ModifySandResource(float amount)
    {
        SandResource += amount;
        _coinAmountText.text = SandResource.ToString();
    }
    public void ModifyFlipResource(float amount)
    {
        FlipResource += amount;
        _flipAmountText.text = SandResource.ToString();
    }
    public IEnumerator CountSand()
    {
        _isCountingSand = true;
        while (_sandResourceToAdd != 0)
        {
            int tick = Mathf.Min(Mathf.Abs(_addAmountPerTick), Mathf.Abs(_sandResourceToAdd)) * Math.Sign(_sandResourceToAdd);
            _sandResourceToAdd -= tick;
            ModifySandResource(tick);
            yield return new WaitForSeconds(0.05f);
        }
        _isCountingSand = false;
    }
    public IEnumerator CountFlip()
    {
        _isCountingFlip = true;
        while (_sandResourceToAdd != 0)
        {
            int tick = Mathf.Min(Mathf.Abs(_addAmountPerTick), Mathf.Abs(_flipResourceToAdd)) * Math.Sign(_flipResourceToAdd);
            _flipResourceToAdd -= tick;
            ModifyFlipResource(tick);
            yield return new WaitForSeconds(0.05f);
        }
        _isCountingFlip = false;
    }
    
}
