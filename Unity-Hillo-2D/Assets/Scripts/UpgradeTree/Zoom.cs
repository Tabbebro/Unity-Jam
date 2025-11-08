using System;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.EventSystems;

public class Zoom : MonoBehaviour, IBeginDragHandler, IDragHandler, IScrollHandler, IEndDragHandler
{
    #region Instance
    public static Zoom Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
        
        _rectTransform = _movable.GetComponent<RectTransform>();
    }
    #endregion

    [Header("Zoom")]
    [SerializeField] private float _zoomSpeed = 0.1f;
    [SerializeField] private float _maxZoom = 10f;
    private Vector3 _initialScale;
    private Vector3 _initialInvertScale;

    [Header("Dragging")]
    [SerializeField] private float _dragSpeed = 0.1f;
    [SerializeField] GameObject _movable;
    private RectTransform _rectTransform;
    private Vector2 _lastMousePosition;
    public bool IsDragging = false;
    void Start()
    {
        _initialScale = transform.localScale;
        _initialInvertScale = InfoBox.Instance.transform.localScale;
    }
    public void OnScroll(PointerEventData eventData)
    {
        var delta = Vector3.one * (eventData.scrollDelta.y * _zoomSpeed);
        var desiredScale = transform.localScale + delta;

        desiredScale = ClampDesiredScale(desiredScale);
        transform.localScale = desiredScale;

        float mainScaleRatio = desiredScale.x / _initialScale.x;
        Vector3 inverseScale = _initialInvertScale / mainScaleRatio;

        InfoBox.Instance.transform.localScale = inverseScale;
    }

    private Vector3 ClampDesiredScale(Vector3 desiredScale)
    {
        desiredScale = Vector3.Max(_initialScale, desiredScale);
        desiredScale = Vector3.Min(_initialScale * _maxZoom, desiredScale);

        return desiredScale;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        IsDragging = true;
        _lastMousePosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 currentMousePosition = eventData.position;
        Vector2 delta = _lastMousePosition - currentMousePosition;
        _rectTransform.anchoredPosition -= delta * _dragSpeed * Time.deltaTime;

        _lastMousePosition = currentMousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsDragging = false;
    }
}
