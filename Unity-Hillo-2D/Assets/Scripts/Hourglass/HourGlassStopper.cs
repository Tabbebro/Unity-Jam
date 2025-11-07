using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HourGlassStopper : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Hourglass _hourglass;
    
    List<GameObject> _possibleBalls = new();
    List<GameObject> _selectedBalls = new();
    List<GameObject> _usedBalls = new();
    bool _ballsGoneThrough = false;

    float _timer = 0f;
    Coroutine _ballDropRoutine;
    private void Start() {
        _hourglass.StartedRotating += ResetStatus;
    }

    private void Update() {
        if (_ballDropRoutine != null || _ballsGoneThrough) { return; }
        _timer += Time.deltaTime;
        if (_timer < _hourglass.Settings.BallCheckInterval) { return; }

        if (_possibleBalls.Count > 0) {
            _timer = 0;
            _ballDropRoutine = StartCoroutine(DropBalls());
        }
        else if (_hourglass.SandParent.childCount == _usedBalls.Count) {
            _ballsGoneThrough = true;
            _hourglass.RotateHourGlass();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // Checks For Correct Layer
        if ((_hourglass.Settings.LayerToCheck & (1 << other.gameObject.layer)) == 0) { return; }

        if (_possibleBalls.Contains(other.gameObject)) { return; }

        other.excludeLayers = 0;
        _possibleBalls.Add(other.gameObject);
    }

    IEnumerator DropBalls() {
        if (_possibleBalls.Count == 0) { _ballDropRoutine = null;  yield break; }

        int BallsThrough;
        if (_possibleBalls.Count < _hourglass.Settings.BallsToLetThrough) {
            BallsThrough = _possibleBalls.Count;
        }
        else {
            BallsThrough = _hourglass.Settings.BallsToLetThrough;
        }


        for (int i = 0; i < BallsThrough; i++) {
            if (_possibleBalls.Count == 0) { _ballDropRoutine = null; yield break; }
            int rndIndex = Random.Range(0, _possibleBalls.Count - 1);
            var ball = _possibleBalls[rndIndex];
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
            if (_hourglass.IsRightSideUp) {
                ball.transform.position = _hourglass.BottomPoint.position;
            }
            else {
                ball.transform.position = _hourglass.TopPoint.position;
            }
            _hourglass.InvokeBallWentThrough(1);
            yield return new WaitForSeconds(_hourglass.Settings.BallDropInterval);
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
