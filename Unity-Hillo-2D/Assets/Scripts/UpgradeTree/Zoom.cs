using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Zoom : MonoBehaviour, IBeginDragHandler, IDragHandler, IScrollHandler
{

    [Header("Zoom")]
    [SerializeField] private float _zoomSpeed = 0.1f;
    [SerializeField] private float _maxZoom = 10f;
    private Vector3 _initialScale;

    [Header("Dragging")]
    [SerializeField] private float _dragSpeed = 0.1f;
    [SerializeField] GameObject _movable;
    private RectTransform _rectTransform;
    private Vector2 _lastMousePosition;
    void Awake()
    {
        _rectTransform = _movable.GetComponent<RectTransform>();
    }
    void Start()
    {
        _initialScale = transform.localScale;
    }
    public void OnScroll(PointerEventData eventData)
    {
        var delta = Vector3.one * (eventData.scrollDelta.y * _zoomSpeed);
        var desiredScale = transform.localScale + delta;

        desiredScale = ClampDesiredScale(desiredScale);

        transform.localScale = desiredScale;
    }

    private Vector3 ClampDesiredScale(Vector3 desiredScale)
    {
        desiredScale = Vector3.Max(_initialScale, desiredScale);
        desiredScale = Vector3.Min(_initialScale * _maxZoom, desiredScale);

        return desiredScale;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _lastMousePosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 currentMousePosition = eventData.position;
        Vector2 delta = _lastMousePosition - currentMousePosition;
        _rectTransform.anchoredPosition -= delta * _dragSpeed * Time.deltaTime;

        _lastMousePosition = currentMousePosition;
    }
}
