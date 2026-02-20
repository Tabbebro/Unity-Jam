using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIRadioButtonUtility : UISelectableUtility<UIRadioButtonSettings, Button>
{
    [Header("Status")]
    [ReadOnly][SerializeField] bool _pressed = false;
    protected enum RadioButtonState{
        Normal,
        Hover,
        Pressed,
        Selected
    }
    [ReadOnly][SerializeField] protected RadioButtonState _radioButtonState = RadioButtonState.Normal;

    protected override void Update() {
        if (_radioButtonState == RadioButtonState.Normal) {
            if (transform.localScale != Vector3.one || transform.localRotation != Quaternion.Euler(Vector3.zero)) {
                SetState(UIState.Normal);
            }
        }
    }

    #region Pointer Events

    public override void OnPointerEnter(PointerEventData eventData) {
        if (!_selectable.interactable) { return; }

        if (_radioButtonState != RadioButtonState.Pressed) {
            SetState(RadioButtonState.Hover);
        }
    }

    public override void OnPointerExit(PointerEventData eventData) {

        if (_radioButtonState != RadioButtonState.Pressed) {
            SetState(IsSelected() ? RadioButtonState.Selected : RadioButtonState.Normal);
        }
    }

    #endregion

    #region State

    protected void SetState(RadioButtonState newState) {
        _radioButtonState = newState;

        KillTransformTween();
        KillColorTweens();

        switch (newState) {
            case RadioButtonState.Normal:
                TweenColor(_settings.TextNormalColor);
                TransformTween(Vector3.zero, Vector3.one);
                break;
            case RadioButtonState.Hover:
                TweenColor(_settings.HighlightedTextColor);
                TransformTween(_settings.HoverTweenRotation, _settings.HoverTweenScale);
                break;
            case RadioButtonState.Pressed:
                TweenColor(_settings.TextSelectedColor);
                TransformTween(_settings.PressTweenRotation, _settings.PressTweenScale);
                break;
            case RadioButtonState.Selected:
                TweenColor(_settings.TextSelectedColor);
                TransformTween(_settings.HoverTweenRotation, _settings.HoverTweenScale);
                break;
            default:
                break;
        }
    }

    bool IsSelected() {
        return _selectable.interactable && EventSystem.current.currentSelectedGameObject == gameObject;
    }

    #endregion

    public void RadioButtonPressed() {

        SetState(RadioButtonState.Pressed);
        ChangeButtonNormalColor(_settings.ButtonPressedColor);

    }

    public void RadioButtonReleased() {
        SetState(IsSelected() ? RadioButtonState.Pressed : RadioButtonState.Normal);
        ChangeButtonNormalColor(IsSelected() ? _settings.ButtonPressedColor : _settings.ButtonDefaultColor);
    }

    void ChangeButtonNormalColor(Color color) {
        ColorBlock block = _selectable.colors;

        block.normalColor = color;

        _selectable.colors = block;
    }
}
