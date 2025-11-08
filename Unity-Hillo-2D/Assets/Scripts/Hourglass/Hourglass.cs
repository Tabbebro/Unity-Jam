using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;
public class Hourglass : MonoBehaviour
{
    public static Hourglass Instance;

    [Header("Settings")]
    public HourGlassSettingsSO Settings;

    [Header("Rotation")]
    public Button RotateButton;
    public Rigidbody2D VisualRB;

    [Header("Sand")]
    public MouseClickNudge Nudge;
    public SandManager SandManager;
    public Transform TopPoint;
    public Transform BottomPoint;

    [Header("Points")]
    public int Points = 0;

    bool _isRotating = false;
    Tween _rotationTween;
    [HideInInspector] public bool IsRightSideUp = true;

    // Events If Needed
    public event Action CanRotate;
    public event Action StartedRotating;
    public event Action FinishedRotating;
    public event Action<int> BallWentThrough;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }

        BallWentThrough += CheckForBalls;
        CanRotate += EnableButton;
        StartedRotating += DisableButton;
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
        BallWentThrough -= CheckForBalls;
        CanRotate -= EnableButton;
        StartedRotating -= DisableButton;
        SandManager.OnAllSandWentThrough -= InvokeCanRotate;
    }

    #region Rotation
    public void RotateHourGlass() {
        if (_isRotating) { return; }
        KillRotationTween();
        _isRotating = true;
        StartedRotating?.Invoke();

        float targetRotation = VisualRB.rotation + 180;
        DOTween.To(() => VisualRB.rotation, x => VisualRB.MoveRotation(x), targetRotation, Settings.TimeForRotation).SetEase(Ease.InOutQuad).OnComplete(FinnishRotation);
    }

    void FinnishRotation() {
        IsRightSideUp = !IsRightSideUp;
        _isRotating = false;
        FinishedRotating?.Invoke();
    }

    void KillRotationTween() {
        if (_rotationTween != null && _rotationTween.IsActive()) {
            _rotationTween.Kill();
            _rotationTween = null;
        }
    }
    #endregion

    public void InvokeBallWentThrough(int value) {
        BallWentThrough?.Invoke(value);
    }

    public void InvokeCanRotate() {
        CanRotate?.Invoke();
    }

    void CheckForBalls(int value) {
        Points += value;
    }

    public void EnableButton() {
        RotateButton.interactable = true;
    }

    public void DisableButton() {
        RotateButton.interactable = false;
    }
}
