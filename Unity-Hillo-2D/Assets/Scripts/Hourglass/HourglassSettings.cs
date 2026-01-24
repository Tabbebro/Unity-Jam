using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "Hourglass/Settings", fileName = "Hourglass Settings")]
public class HourglassSettings : ScriptableObject
{
    [Header("Flow Values")]
    public float FlowPerSecond = 20;

    [Header("Rotation Values")]
    public float RotationDuration = 2;
    public Ease RotationEase;

    [Header("Auto Rotation")]
    public bool AutoRotationUnlocked = false;
    public float AutoRotateInterval = 2;
}
