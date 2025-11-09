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

    [Header("Sign")]
    [SerializeField] GameObject _sign;
    Tween _signTween;

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
    public event Action OnBallWentThrough;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }

        OnBallWentThrough += CheckForSign;
        OnRotationEnabled += EnableButton;
        OnRotationStarted += DisableButton;
        OnRotationStarted += PlayeRotateAudio;
        SandManager.OnAllSandWentThrough += InvokeCanRotate;
        SandManager.CancelAllSandWentThrough += InvokeCancelRotate;
        OnRotationFinished += SpawnSand;
        
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
        OnBallWentThrough -= CheckForSign;
        OnRotationEnabled -= EnableButton;
        OnRotationStarted -= DisableButton;
        OnRotationStarted -= PlayeRotateAudio;
        SandManager.OnAllSandWentThrough -= InvokeCanRotate;
        SandManager.CancelAllSandWentThrough -= InvokeCancelRotate;
        OnRotationFinished -= SpawnSand;

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

    public void SpawnSand() {
        //Debug.Log("Test");
        SandManager.SpawnRandomSand(Settings.AmountOfSandSpawnedOnRotation);
    }

    public void InvokeBallWentThrough() {
        OnBallWentThrough?.Invoke();
    }

    public void InvokeCanRotate() {
        OnRotationEnabled?.Invoke();
    }
    private void InvokeCancelRotate()
    {
        DisableButton();
    }
    

    void CheckForSign() {
        if (!_sign.activeInHierarchy || _signTween != null) { return; }

        Vector3 endPosition = _sign.transform.position + new Vector3(-400, -400, 0);
        _sign.transform.DOMove(new Vector3(480, 540, 0), 1f).SetEase(Ease.OutQuad).OnComplete(RotateDude);
    }

    void RotateDude() {
        Vector3 endPosition = _sign.transform.position + new Vector3(2000, 2000, 0);
        _sign.transform.DOScale(Vector3.zero, 2f).SetEase(Ease.OutQuad);
        _sign.transform.DORotate(new Vector3(0,0,180), 1f).SetEase(Ease.OutQuad).OnComplete(RotateAgain);
        //_sign.SetActive(false);
    }

    void RotateAgain() {
        _sign.transform.DORotate(new Vector3(0,0,360), 1f).SetEase(Ease.OutQuad).OnComplete(CloseDude);
    }

    void CloseDude() {
        _sign.SetActive(false);
    }

    public void EnableButton() {
        CanRotate = true;
        RotateButton.interactable = true;
    }

    public void DisableButton() {
        //print("Disable rotate button");
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
            Settings.BallsToFlowThrough = 1;
        }
        if (name.ToString() == "LetThroughPerSecond")
        {
            Settings.BallsToFlowThrough = Mathf.RoundToInt(Settings.BallsToFlowThrough * item.UpgradeMultiplier);
        }
        if (name.ToString() == "LetThroughInterval") {
            Settings.BallFlowInterval *= item.UpgradeMultiplier;
        }
        if (name.ToString() == "LetThroughCheckInterval") {
            Settings.FlowCheckInterval *= item.UpgradeMultiplier;
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
        if (name.ToString() == "NudgeFlow") {
            Settings.BallsNudgeLetThrough += item.UpgradeAmount;
        }
        if (name.ToString() == "AmountOfSandToSpawnPerRotation") {
            Settings.AmountOfSandSpawnedOnRotation += item.UpgradeAmount;
        }
        
    }

    
    
}
