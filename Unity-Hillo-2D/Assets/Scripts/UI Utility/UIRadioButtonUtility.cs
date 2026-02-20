using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIRadioButtonUtility : UISelectableUtility<UIRadioButtonSettings, Button>
{
    [Header("Status")]
    [ReadOnly][SerializeField] bool _pressed = false;

    #region Pointer Events
    public override void OnPointerEnter(PointerEventData eventData) {
        if (!_selectable.interactable) { return; }

        TweenColor(_settings.HighlightedTextColor);
        TransformTween(_settings.TweenRotation, _settings.TweenScale);
    }

    public override void OnPointerExit(PointerEventData eventData) {
        if (_pressed) {
            TweenColor(_settings.TextSelectedColor);
        }
        else {
            TweenColor(_settings.TextNormalColor);
            TransformTween(Vector3.zero, Vector3.one);
        }
    }
    #endregion

    public void RadioButtonPressed() {
        _pressed = true;

        ChangeButtonNormalColor(_settings.ButtonPressedColor);

        if (EventSystem.current.currentSelectedGameObject == gameObject) { 
            return; 
        }
        TweenColor(_settings.TextSelectedColor);
        TransformTween(_settings.TweenRotation, _settings.TweenScale);
    }

    public void RadioButtonReleased() {
        _pressed = false;

        ChangeButtonNormalColor(_settings.ButtonDefaultColor);

        if (EventSystem.current.currentSelectedGameObject == gameObject) { 
            return; 
        }

        TweenColor(_settings.TextNormalColor);
        TransformTween(Vector3.zero, Vector3.one);
    }

    void ChangeButtonNormalColor(Color color) {
        ColorBlock block = _selectable.colors;

        block.normalColor = color;

        _selectable.colors = block;
    }
}
