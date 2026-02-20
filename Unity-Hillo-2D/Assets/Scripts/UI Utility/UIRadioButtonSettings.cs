using UnityEngine;

[CreateAssetMenu(fileName = "UI Radio Button Settings", menuName = "UI/Radio Button Settings")]
public class UIRadioButtonSettings : UISelectableSettings
{
    public Color HighlightedTextColor;

    [Header("Button Color")]
    public Color ButtonPressedColor = Color.white;
    public Color ButtonDefaultColor;
}
