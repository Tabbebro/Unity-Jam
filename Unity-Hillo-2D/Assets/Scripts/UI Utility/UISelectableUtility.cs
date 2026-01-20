using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using ScrutableObjects;

public class UISelectableUtility : UISelectableUtility<UISelectableSettings, Selectable> { }

public abstract class UISelectableUtility<TSettings, TSelectable> : MonoBehaviour, 
    IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IMoveHandler
    where TSettings : UISelectableSettings
    where TSelectable : Selectable
{

    [Header("Settings")]
    [SerializeField] [ShowProperties] protected TSettings _settings;

    [Header("Refs")]
    [SerializeField] GameObject _objectToTween;
    [SerializeField] protected List<TextMeshProUGUI> _texts;

    protected TSelectable _selectable;
    protected RectTransform _tweenableRect;
    protected List<Tween> _colorTweens = new();
    protected List<Tween> _transformTweens = new();

    protected virtual void Awake() {
        _selectable = GetComponent<TSelectable>();
        SetTweenableSelectable();
    }

    protected virtual void OnEnable() {
        if (EventSystem.current.currentSelectedGameObject == _selectable.gameObject) {
            SetTextColors(_settings.TextSelectedColor);
            TransformTween(_settings.TweenRotation, _settings.TweenScale);
        }
        else {
            SetTextColors(_settings.TextNormalColor);
            ResetTransformTweenValues();
        }
    }

    protected virtual void OnDisable() {
        KillColorTweens();
        KillTransformTween();

        ResetTransformTweenValues();
        SetTextColors(_settings.TextNormalColor);
    }

    protected virtual void OnDestroy() {
        KillColorTweens();
        KillTransformTween();
    }

    #region Pointer Events
    public virtual void OnPointerEnter(PointerEventData eventData) {
        if (!Cursor.visible || Cursor.lockState == CursorLockMode.Locked) { return; }
        _selectable.Select();
    }

    public virtual void OnPointerExit(PointerEventData eventData) {
        EventSystem.current.SetSelectedGameObject(null);
    }
    #endregion

    #region Select Events
    public virtual void OnSelect(BaseEventData eventData) {
        if (!_selectable.interactable) { return; }

        TweenColor(_settings.TextSelectedColor);
        TransformTween(_settings.TweenRotation, _settings.TweenScale);
    }

    public virtual void OnDeselect(BaseEventData eventData) {
        TweenColor(_settings.TextNormalColor);
        TransformTween(Vector3.zero, Vector3.one);
    }
    #endregion

    #region OnMoveEvent
    public virtual void OnMove(AxisEventData eventData) {
        
    }
    #endregion

    #region Transform Tweening

    public virtual void SetTweenableSelectable() {
        if (_objectToTween.TryGetComponent<RectTransform>(out RectTransform rect)) {
            _tweenableRect = rect;
        }
        else {
            Debug.LogError($"[<color=#f54c7f>{this.name}</color>]" +
                $"<color=orange> Couldn't Get RectTransform Of Selectable: </color>" + _selectable.gameObject.name);
        }
    }

    protected virtual void TransformTween(Vector3 targetRotation, Vector3 targetScale) {
        if (_tweenableRect == null) { SetTweenableSelectable(); }

        KillTransformTween();

        // Rotation
        _transformTweens.Add(_tweenableRect.transform.DOLocalRotate(targetRotation, _selectable.colors.fadeDuration).SetEase(_settings.TweenEase).SetUpdate(true));

        // Scale Tween
        _transformTweens.Add(_tweenableRect.transform.DOScale(targetScale, _selectable.colors.fadeDuration).SetEase(_settings.TweenEase).SetUpdate(true));
    }

    protected virtual void ResetTransformTweenValues() {
        if (_tweenableRect == null) { SetTweenableSelectable(); }
        
        KillTransformTween();

        _tweenableRect.transform.localRotation = Quaternion.Euler(Vector3.zero);
        _tweenableRect.transform.localScale = Vector3.one;
    }

    protected virtual void KillTransformTween() {
        foreach (var t in _transformTweens) {
            t?.Kill();
        }

        _transformTweens.Clear();
    }

    #endregion

    #region Color Tweening

    protected virtual void TweenColor(Color targetColor) {
        KillColorTweens();

        foreach (var text in _texts) {
            Tween t = text.DOColor(targetColor, _selectable.colors.fadeDuration).SetEase(_settings.TweenEase).SetUpdate(true);
            _colorTweens.Add(t);
        }
    }

    protected virtual void SetTextColors(Color color) {
        foreach (var text in _texts) {
            text.color = color;
        }
    }

    void KillColorTweens() {
        foreach (var t in _colorTweens) {
            t?.Kill();
        }

        _colorTweens.Clear();
    }

    #endregion
}
