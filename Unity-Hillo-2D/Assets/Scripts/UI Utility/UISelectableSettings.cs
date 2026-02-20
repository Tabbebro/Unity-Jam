using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "UI Selection Settings", menuName = "UI/Selectable Settings")]
public class UISelectableSettings : ScriptableObject
{
    [Header("Hover Tween Settings")]
    public Vector3 HoverTweenRotation = Vector3.zero;
    public Vector3 HoverTweenScale = new(1.05f, 1.05f, 1.05f);
    public Ease HoverTweenEase = Ease.OutQuad;

    [Header("Press Tween Settings")]
    public Vector3 PressTweenRotation = Vector3.zero;
    public Vector3 PressTweenScale = new(1.05f, 1.05f, 1.05f);
    public float PressTweenDuration = 0.1f;
    public Ease PressTweenEase = Ease.OutQuad;

    [Header("Color")]
    public Color TextSelectedColor = Color.white;
    public Color TextNormalColor;
}
