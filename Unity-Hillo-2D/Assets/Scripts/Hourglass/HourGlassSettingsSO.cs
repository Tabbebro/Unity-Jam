using UnityEngine;

[CreateAssetMenu(menuName = "Hourglass/Settings", fileName = "Hourglass Settings")]
public class HourGlassSettingsSO : ScriptableObject
{
    [Header("Normal Flow")]
    [Min(0)] public int BallsToFlowThrough = 5;
    [Min(0)] public float BallFlowInterval = 0.2f;
    [Min(0)] public float FlowCheckInterval = 2f;

    [Header("Nudge")]
    [Min(0)] public int BallsNudgeLetThrough = 1;
}
