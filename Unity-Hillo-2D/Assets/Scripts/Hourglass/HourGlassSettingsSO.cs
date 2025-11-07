using UnityEngine;

[CreateAssetMenu(menuName = "Hourglass/Settings", fileName = "Hourglass Settings")]
public class HourGlassSettingsSO : ScriptableObject
{
    [Header("Normal Flow")]
    public int BallsToFlowThrough = 5;
    public float BallFlowInterval = 0.2f;
    public float FlowCheckInterval = 2f;

    [Header("Nudge")]
    public int BallsNudgeLetThrough = 1;
}
