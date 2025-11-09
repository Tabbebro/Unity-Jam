using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;
using ScrutableObjects;
public class Hourglass : MonoBehaviour
{
    public static Hourglass Instance;

    [HideInInspector] public bool IsRightSideUp = true;
    
    [Header("Settings")]
    [ShowProperties] public HourGlassSettingsSO Settings;


    [Header("Points")]
    public int Points = 0;

    [Header("Rotation")]
    public bool IsRotating = false;
    public bool CanRotate = false;
    public Button RotateButton;
    public Rigidbody2D VisualRB;
    Tween _rotationTween;

    [Header("Sand")]
    public MouseClickNudge Nudge;
    public SandManager SandManager;
    public Transform TopPoint;
    public Transform BottomPoint;

    [Header("Audio")]
    [SerializeField] AudioClip _rotateClip;
    [SerializeField] AudioSource _audioSource;


    // Events If Needed
    public event Action OnRotationEnabled;
    public event Action OnRotationStarted;
    public event Action OnRotationFinished;
    public event Action<int> OnBallWentThrough;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }

        OnBallWentThrough += CheckForBalls;
        OnRotationEnabled += EnableButton;
        OnRotationStarted += DisableButton;
        OnRotationStarted += PlayeRotateAudio;
        SandManager.OnAllSandWentThrough += InvokeCanRotate;
        SandManager.CancelAllSandWentThrough += InvokeCancelRotate;
        
    }
    void Start()
    {
        UpgradeManager.Instance.UpgradeHappened += NewUpgrade;

        Settings = Instantiate(Settings);
        
    }


    private void OnEnable() {
        KillRotationTween();
    }

    private void OnDisable() {
        KillRotationTween();
    }

    private void OnDestroy() {
        KillRotationTween();
        OnBallWentThrough -= CheckForBalls;
        OnRotationEnabled -= EnableButton;
        OnRotationStarted -= DisableButton;
        OnRotationStarted -= PlayeRotateAudio;
        SandManager.OnAllSandWentThrough -= InvokeCanRotate;
    }

    #region Rotation
    public void RotateHourGlass() {
        if (IsRotating) { return; }
        KillRotationTween();
        IsRightSideUp = !IsRightSideUp;
        IsRotating = true;
        OnRotationStarted?.Invoke();

        float targetRotation = VisualRB.rotation + 180;
        DOTween.To(() => VisualRB.rotation, x => VisualRB.MoveRotation(x), targetRotation, Settings.RotationSpeed).SetEase(Ease.InOutQuad).OnComplete(FinnishRotation);
    }

    void FinnishRotation() {
        IsRotating = false;
        OnRotationFinished?.Invoke();
        UpgradeManager.Instance.ModifyFlipResource(1);
    }

    void KillRotationTween() {
        if (_rotationTween != null && _rotationTween.IsActive()) {
            _rotationTween.Kill();
            _rotationTween = null;
        }
    }
    #endregion

    public void InvokeBallWentThrough(int value) {
        OnBallWentThrough?.Invoke(value);
    }

    public void InvokeCanRotate() {
        OnRotationEnabled?.Invoke();
    }
    private void InvokeCancelRotate()
    {
        DisableButton();
    }
    

    void CheckForBalls(int value) {
        Points += value;
    }

    public void EnableButton() {
        CanRotate = true;
        RotateButton.interactable = true;
    }

    public void DisableButton() {
        print("Disable rotate button");
        CanRotate = false;
        RotateButton.interactable = false;
    }

    public void PlayeRotateAudio() {
        _audioSource.pitch = _rotateClip.length / Settings.RotationSpeed;
        _audioSource.PlayOneShot(_rotateClip);
    }
    
    
    private void NewUpgrade(object name, VariableInfo item)
    {
        if (name.ToString() == "LetThroughUnlocked")
        {
            print("ballflowthrough unlocked");
            Settings.BallsToFlowThrough = 1;
        }
        if (name.ToString() == "LetThroughPerSecond")
        {
            Settings.BallsToFlowThrough = Mathf.RoundToInt(Settings.BallsToFlowThrough * item.UpgradeMultiplier);
        }
        if (name.ToString() == "RotationSpeed") {
            Settings.RotationSpeed *= item.UpgradeMultiplier;
        }
        if (name.ToString() == "AutomaticRotationUnlocked") {
            if (!Settings.AutomaticRotationUnlocked) {
                Settings.AutomaticRotationUnlocked = item.UpgradeBool;
            }
        }
        if (name.ToString() == "AutomaticRotationCooldown") {
            Settings.AutomaticRotationTime *= item.UpgradeMultiplier;
        }
    }

    
    
}
