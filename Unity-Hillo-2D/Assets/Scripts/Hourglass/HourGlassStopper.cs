using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HourGlassStopper : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Hourglass _hourglass;

    [Header("Layer Mask")]
    [SerializeField] LayerMask _layerToCheck;

    List<GameObject> _possibleBalls = new();
    List<GameObject> _selectedBalls = new();
    List<GameObject> _nudgedBalls = new();
    List<GameObject> _usedBalls = new();
    bool _ballsGoneThrough = false;

    float _timer = 0f;
    Coroutine _ballFlowRoutine;
    void Start() {
        _hourglass.StartedRotating += ResetStatus;
        _hourglass.Nudge.OnSandNudged += Nudged;
    }

    void Update() {
        if (_ballFlowRoutine != null || _ballsGoneThrough) { return; }
        _timer += Time.deltaTime;
        if (_timer < _hourglass.Settings.FlowCheckInterval) { return; }

        if (_possibleBalls.Count > 0) {
            _timer = 0;
            _ballFlowRoutine = StartCoroutine(BallFlow());
        }
        else if (_hourglass.SandParent.childCount == _usedBalls.Count) {
            _ballsGoneThrough = true;
            _hourglass.RotateHourGlass();
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        // Checks For Correct Layer
        if ((_layerToCheck & (1 << other.gameObject.layer)) == 0) { return; }

        if (_possibleBalls.Contains(other.gameObject)) { return; }

        _possibleBalls.Add(other.gameObject);
    }

    void OnTriggerExit2D(Collider2D other) {
        // Checks For Correct Layer
        if ((_layerToCheck & (1 << other.gameObject.layer)) == 0) { return; }

        if (_possibleBalls.Contains(other.gameObject)) { _possibleBalls.Remove(other.gameObject); }
    }

    IEnumerator BallFlow() {
        if (_possibleBalls.Count == 0) { _ballFlowRoutine = null;  yield break; }

        List<GameObject> newBall = GetBall(_hourglass.Settings.BallsToFlowThrough);
        if (newBall?.Count == 0) { _ballFlowRoutine = null; yield break; }
        _selectedBalls = newBall;

        yield return SpawnBalls(_selectedBalls);

        _selectedBalls.Clear();
        _ballFlowRoutine = null;
    }

    void ResetStatus() {
        _possibleBalls.Clear();
        _usedBalls.Clear();
        _ballsGoneThrough = false;
        _timer = 0;
    }

    void Nudged() {
        StartCoroutine(DropNudgedBalls());
    }

    IEnumerator DropNudgedBalls() {
        List<GameObject> newBall = GetBall(_hourglass.Settings.BallsNudgeLetThrough);
        if (newBall?.Count == 0) { yield break; }
        _nudgedBalls = newBall;

        yield return SpawnBalls(_nudgedBalls);
        
        _nudgedBalls.Clear();
    }

    List<GameObject> GetBall(int amountOfBalls) {
        List<GameObject> newBalls = new();
        if (_possibleBalls.Count <= 0) { return null; }

        int ballsThrough;
        if (_possibleBalls.Count < amountOfBalls) {
            ballsThrough = _possibleBalls.Count;
        }
        else {
            ballsThrough = amountOfBalls;
        }

        for (int i = 0; i < ballsThrough; i++) {
            if (_possibleBalls.Count == 0) { return null; }
            var ball = _possibleBalls[Random.Range(0, _possibleBalls.Count - 1)];
            if (!_usedBalls.Contains(ball) || _nudgedBalls.Contains(ball)) {
                _usedBalls.Add(ball);
                newBalls.Add(ball);
            }
            else {
                i--;
            }
            _possibleBalls.Remove(ball);
        }

        return newBalls;
    }

    IEnumerator SpawnBalls(List<GameObject> ballList) {
        foreach (var ball in ballList) {
            if (ball == null) { continue; }
            if (_hourglass.IsRightSideUp) {
                ball.transform.position = _hourglass.BottomPoint.position;
            }
            else {
                ball.transform.position = _hourglass.TopPoint.position;
            }
            _hourglass.InvokeBallWentThrough(1);
            yield return new WaitForSeconds(_hourglass.Settings.BallFlowInterval);
        }
    }
}
