using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HourGlassStopper : MonoBehaviour
{
    [Header("Teleport Points")]
    [SerializeField] Transform _topPoint;
    [SerializeField] Transform _bottomPoint;

    [Header("Layer Masks")]
    [SerializeField] LayerMask _layerToCheck;

    [Header("Balls In The Field")]
    List<GameObject> _possibleBalls = new();
    List<GameObject> _selectedBalls = new();
    List<GameObject> _usedBalls = new();
    bool _ballsGoneThrough = false;

    [Header("Settings")]
    [SerializeField] int _ballsToLetThrough = 5;
    [SerializeField] float _ballDropInterval = 0.2f;
    [SerializeField] float _ballCheckInterval = 2f;
    [SerializeField] float _timer = 0f;

    [Header("Sand Parent")]
    [SerializeField] GameObject _sandParent;

    Coroutine _ballDropRoutine;
    private void Start() {
        Hourglass.Instance.StartedRotating += ResetStatus;
    }

    private void Update() {
        if (_ballDropRoutine != null || _ballsGoneThrough) { return; }
        _timer += Time.deltaTime;
        if (_timer < _ballCheckInterval) { return; }

        if (_possibleBalls.Count > 0) {
            _timer = 0;
            _ballDropRoutine = StartCoroutine(DropBalls());
        }
        else if (_sandParent.transform.childCount == _usedBalls.Count) {
            _ballsGoneThrough = true;
            Hourglass.Instance.RotateHourGlass();
            Debug.Log("All Balls Gone Through");
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // Checks For Correct Layer
        if ((_layerToCheck & (1 << other.gameObject.layer)) == 0) { return; }

        if (_possibleBalls.Contains(other.gameObject)) { return; }

        other.excludeLayers = 0;
        _possibleBalls.Add(other.gameObject);
    }

    IEnumerator DropBalls() {
        if (_possibleBalls.Count == 0) { _ballDropRoutine = null;  yield break; }

        int BallsThrough;
        if (_possibleBalls.Count < _ballsToLetThrough) {
            BallsThrough = _possibleBalls.Count;
        }
        else {
            BallsThrough = _ballsToLetThrough;
        }


        for (int i = 0; i < BallsThrough; i++) {
            var ball = _possibleBalls[Random.Range(0, _possibleBalls.Count - 1)];
            if (!_usedBalls.Contains(ball)) {
                _selectedBalls.Add(ball);
                _usedBalls.Add(ball);
            }
            else {
                i--;
            }
            _possibleBalls.Remove(ball);
        }

        foreach (var ball in _selectedBalls) {
            if (Hourglass.Instance.IsRightSideUp) {
                ball.transform.position = _bottomPoint.position;
            }
            else {
                ball.transform.position = _topPoint.position;
            }
                yield return new WaitForSeconds(_ballDropInterval);
        }

        _selectedBalls.Clear();
        _ballDropRoutine = null;
    }

    void ResetStatus() {
        _possibleBalls.Clear();
        _usedBalls.Clear();
        _ballsGoneThrough = false;
        _timer = 0;
    }
}
