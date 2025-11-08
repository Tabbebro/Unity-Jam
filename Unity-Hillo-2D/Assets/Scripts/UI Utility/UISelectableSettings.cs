using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "UI Selection Settings", menuName = "UI/Selectable Settings")]
public class UISelectableSettings : ScriptableObject
{
    [Header("Tween Settings")]
    public Vector3 TweenRotation = Vector3.zero;
    public Vector3 TweenScale = new(1.05f, 1.05f, 1.05f);
    public Ease TweenEase = Ease.OutQuad;

    [Header("Color")]
    public Color TextSelectedColor = Color.white;
    public Color TextNormalColor;
}
