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
    public bool EnoughResource(float amount) => SandResource >= amount;
    public bool EnoughFlipResource(float amount) => FlipResource >= amount;
    public void ModifySandResource(float amount) => SandResource += amount;
    public void ModifyFlipResource(float amount) => FlipResource += amount;
}
