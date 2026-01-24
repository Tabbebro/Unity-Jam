using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HourglassVisuals : MonoBehaviour
{
    [Header("Refs")]
    public HourglassLogic Logic;

    [Header("\"Sand\" Fill")]
    [SerializeField] Image _topImage;
    [SerializeField] Image _bottomImage;

    [Header("Rotation")]
    [SerializeField] Transform _hourglassVisual;
    Quaternion _uprightRotation;
    Quaternion _invertedRotation;
    Tween _rotationTween;

    [Header("Values")]
    [ReadOnly][SerializeField] FlowDirection _direction;

    private void Awake() {
        _uprightRotation = Quaternion.identity;
        _invertedRotation = Quaternion.Euler(0,0,180);
    }

    private void Start() {
        Logic.OnSandFlowed += UpdateSandFlowVisual;
        Logic.OnRotated += RotateHourglassVisual;
    }

    private void OnDestroy() {
        Logic.OnSandFlowed -= UpdateSandFlowVisual;
        Logic.OnRotated -= RotateHourglassVisual;
    }

    void UpdateSandFlowVisual(double amount, FlowDirection direction) {

        double total = Logic.State.TotalSand;
        if (total <= 0) { return; }

        CheckDirection(direction);

        float topRatio = (float)(Logic.State.SandTop / total);
        float bottomRatio = 1f - topRatio;

        _topImage.fillAmount = topRatio;
        _bottomImage.fillAmount = bottomRatio;
    }

    void RotateHourglassVisual(FlowDirection direction) {
        CheckDirection(direction);
        
        HourglassSettings settings = Logic.State.Settings;

        // Kill Tween
        _rotationTween?.Kill();

        // Get Target Rotation
        Quaternion target = direction == FlowDirection.TopToBottom 
            ? _uprightRotation 
            : _invertedRotation;

        // Animate Rotation
        _rotationTween = _hourglassVisual.DORotateQuaternion(target, settings.RotationDuration).SetEase(settings.RotationEase);
    }

    void CheckDirection(FlowDirection direction) {
        if (_direction == direction) { return; }

        _direction = direction;
        int fillOrigin;

        if (direction == FlowDirection.TopToBottom) { fillOrigin = 0; }
        else { fillOrigin = 1; }

        _topImage.fillOrigin = fillOrigin;
        _bottomImage.fillOrigin = fillOrigin;
    }
}
