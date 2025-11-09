using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HourGlassStopper : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Hourglass _hourglass;

    [Header("Layer Mask")]
    [SerializeField] LayerMask _layerToCheck;

    List<GameObject> _possibleBalls = new();
    List<GameObject> _selectedBalls = new();
    List<GameObject> _nudgedBalls = new();
    HashSet<GameObject> _usedBalls = new();
    [SerializeField] bool _ballsCanGoThrough = true;
    bool _ballsGoneThrough = false;
    float _timer = 0f;

    [Header("Auto Rotation Timer")]
    [SerializeField] GameObject _rotationTimerMaskImage;
    [SerializeField] Image _RotationTimerFill;

    [Header("Audio")]
    [SerializeField] List<AudioClip> _sandClips;
    [SerializeField] AudioSource _audioSource;

    Coroutine _ballFlowRoutine;
    void Start() {
        _hourglass.OnRotationStarted += ResetStatus;
        _hourglass.Nudge.OnSandNudged += Nudged;
        _hourglass.OnRotationStarted += ResetAutoRotation;
    }

    private void OnDestroy() {
        _hourglass.OnRotationStarted -= ResetStatus;
        _hourglass.Nudge.OnSandNudged -= Nudged;
        _hourglass.OnRotationStarted -= ResetAutoRotation;
    }

    void Update() {
        if (!_ballsCanGoThrough || _hourglass.IsRotating) { return; }
        if (_hourglass.Settings.AutomaticRotationUnlocked && _hourglass.CanRotate && !_hourglass.IsRotating) {
            if (!_rotationTimerMaskImage.activeInHierarchy) { _rotationTimerMaskImage.SetActive(true); }

            _RotationTimerFill.fillAmount = _timer / _hourglass.Settings.AutomaticRotationTime;

            if (_timer >= _hourglass.Settings.AutomaticRotationTime) {
                _hourglass.RotateHourGlass();
            }
        }
        if (_ballFlowRoutine != null || _ballsGoneThrough) { return; }
        _timer += Time.deltaTime;
        if (_timer < _hourglass.Settings.FlowCheckInterval) { return; }

        if (_possibleBalls.Count > 0) {
            _timer = 0;
            _ballFlowRoutine = StartCoroutine(BallFlow());
        }
        // else if (_timer >= _hourglass.Settings.RotationFailSafeTimer && !_hourglass.CanRotate && !_hourglass.IsRotating) {
        //     _timer = 0;
        //     _hourglass.InvokeCanRotate();
        // }
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
        if (newBall == null || newBall.Count == 0) { _ballFlowRoutine = null; yield break; }
        _selectedBalls = newBall;

        yield return SpawnBalls(_selectedBalls);

        _selectedBalls.Clear();
        _ballFlowRoutine = null;
    }

    void ResetStatus() {
        _possibleBalls.Clear();
        _selectedBalls.Clear();
        _nudgedBalls.Clear();
        _usedBalls.Clear();
        _ballsGoneThrough = false;
        _timer = 0;
    }

    void Nudged() {
        StartCoroutine(DropNudgedBalls());
    }

    IEnumerator DropNudgedBalls() {
        List<GameObject> newBall = GetBall(_hourglass.Settings.BallsNudgeLetThrough);
        if (newBall == null || newBall.Count == 0) { yield break; }
        _nudgedBalls = newBall;

        yield return SpawnBalls(_nudgedBalls);
        
        _nudgedBalls.Clear();
    }

    List<GameObject> GetBall(int amountOfBalls) {

        _possibleBalls.RemoveAll(b => b == null);
        if (_possibleBalls.Count <= 0) { return null; }

        List<GameObject> newBalls = new();
        int ballsThrough = Mathf.Min(amountOfBalls, _possibleBalls.Count);
        int safety = 0;

        while (newBalls.Count < ballsThrough && _possibleBalls.Count > 0 && safety < ballsThrough * 10) {
            safety++;
            var ball = _possibleBalls[Random.Range(0, _possibleBalls.Count)];
            _possibleBalls.Remove(ball);

            if (!_usedBalls.Contains(ball) && !_selectedBalls.Contains(ball) && !_nudgedBalls.Contains(ball)) {
                _usedBalls.Add(ball);
                newBalls.Add(ball);
            }
        }

        return newBalls;
    }

    IEnumerator SpawnBalls(List<GameObject> ballList) {
        if (ballList == null || ballList.Count == 0) { yield break; }

        var ballsCopy = new List<GameObject>(ballList);

        foreach (var ball in ballsCopy) {

            if (ball == null) { continue; }

            if (_hourglass.IsRightSideUp) { ball.transform.position = _hourglass.BottomPoint.position; }
            else { ball.transform.position = _hourglass.TopPoint.position; }

            PlaySandAudio();
            _hourglass.InvokeBallWentThrough(1);
            yield return new WaitForSeconds(_hourglass.Settings.BallFlowInterval);
        }
    }

    void PlaySandAudio() {
        float randomVolume = Random.Range(0.5f, 1f);
        AudioClip randomClip = _sandClips[Random.Range(0, _sandClips.Count)];
        _audioSource.PlayOneShot(randomClip, randomVolume);
    }

    public void ResetAutoRotation() {
        _timer = 0;
        _RotationTimerFill.fillAmount = 0;
        _rotationTimerMaskImage.SetActive(false);
    }
}
