using UnityEngine;

[CreateAssetMenu(menuName = "Hourglass/Settings", fileName = "Hourglass Settings")]
public class HourGlassSettingsSO : ScriptableObject
{
    [Header("Settings")]
    public LayerMask LayerToCheck;
    public int BallsToLetThrough = 5;
    public float BallDropInterval = 0.2f;
    public float BallCheckInterval = 2f;
}
