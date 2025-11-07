using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
public class Hourglass : MonoBehaviour
{
    public static Hourglass Instance;

    [Header("Debug")]
    public bool Rotate = false;

    [Header("Points")]
    public int Points = 0;

    Rigidbody2D _rb;
    bool _isRotating = false;
    Tween _rotationTween;

    // Events If Needed
    public event Action StartedRotating;
    public event Action FinishedRotating;

    List<GameObject> _ballsGoneThrough = new();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
        _rb = GetComponent<Rigidbody2D>();

        FinishedRotating += ClearBallList;
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

    private void Update() {
        if (Rotate) {
            Rotate = false;
            RotateHourGlass();
        }
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

    private void OnTriggerEnter2D(Collider2D collision) {
        if (_ballsGoneThrough.Contains(collision.gameObject)) { return; }
        _ballsGoneThrough.Add(collision.gameObject);
        Points++;
    }

    void ClearBallList() {
        _ballsGoneThrough.Clear();
    }
}
