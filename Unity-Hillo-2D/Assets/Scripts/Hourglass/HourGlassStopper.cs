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
    [SerializeField] List<GameObject> _possibleBalls = new();
    [SerializeField] List<GameObject> _selectedBalls = new();

    [Header("Settings")]
    [SerializeField] int _ballsToLetThrough = 5;
    [SerializeField] float _ballDropInterval = 0.2f;
    [SerializeField] float _ballCheckInterval = 2f;
    [SerializeField] float _timer = 0f;

    Coroutine _ballDropRoutine;
    private void Update() {
        if (_ballDropRoutine != null) { return; }
        _timer += Time.deltaTime;
        if (_timer < _ballCheckInterval) { return; }

        _timer = 0;
        _ballDropRoutine = StartCoroutine(DropBalls());
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

        for (int i = 0; i < _ballsToLetThrough; i++) {
            var ball = _possibleBalls[Random.Range(0, _possibleBalls.Count)];
            _possibleBalls.Remove(ball);
            _selectedBalls.Add(ball);
        }

        foreach (var ball in _selectedBalls) {
            ball.transform.position = _bottomPoint.position;
            yield return new WaitForSeconds(_ballDropInterval);
        }

        _selectedBalls.Clear();
        _ballDropRoutine = null;
    }
}
