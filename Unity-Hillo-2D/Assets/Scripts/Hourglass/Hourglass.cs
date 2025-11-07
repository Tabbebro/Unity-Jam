using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
public class Hourglass : MonoBehaviour
{
    public static Hourglass Instance;

    [Header("Settings")]
    public HourGlassSettingsSO Settings;

    [Header("Refs")]
    public MouseClickNudge Nudge;
    public Transform TopPoint;
    public Transform BottomPoint;
    public Transform SandParent;

    [Header("Points")]
    public int Points = 0;

    Rigidbody2D _rb;
    bool _isRotating = false;
    Tween _rotationTween;
    [HideInInspector] public bool IsRightSideUp = true;

    // Events If Needed
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
        _rb = GetComponent<Rigidbody2D>();

        BallWentThrough += CheckForBalls;
    }

    private void OnEnable() {
        KillRotationTween();
    }

    private void OnDisable() {
        KillRotationTween();
    }

    private void OnDestroy() {
        KillRotationTween();
    }

    #region Rotation
    public void RotateHourGlass() {
        if (_isRotating) { return; }
        KillRotationTween();
        _isRotating = true;
        StartedRotating?.Invoke();

        float targetRotation = _rb.rotation + 180;
        DOTween.To(() => _rb.rotation, x => _rb.MoveRotation(x), targetRotation, 2f).SetEase(Ease.InOutQuad).OnComplete(FinnishRotation);
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

    void CheckForBalls(int value) {
        Points += value;
    }
}
