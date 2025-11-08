using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;
public class Hourglass : MonoBehaviour
{
    public static Hourglass Instance;

    [HideInInspector] public bool IsRightSideUp = true;
    
    [Header("Settings")]
    public HourGlassSettingsSO Settings;

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
        DOTween.To(() => VisualRB.rotation, x => VisualRB.MoveRotation(x), targetRotation, Settings.TimeForRotation).SetEase(Ease.InOutQuad).OnComplete(FinnishRotation);
    }

    void FinnishRotation() {
        IsRotating = false;
        OnRotationFinished?.Invoke();
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

    void CheckForBalls(int value) {
        Points += value;
    }

    public void EnableButton() {
        CanRotate = true;
        RotateButton.interactable = true;
    }

    public void DisableButton() {
        CanRotate = false;
        RotateButton.interactable = false;
    }

    public void PlayeRotateAudio() {
        _audioSource.pitch = _rotateClip.length / Settings.TimeForRotation;
        _audioSource.PlayOneShot(_rotateClip);
    }
}
