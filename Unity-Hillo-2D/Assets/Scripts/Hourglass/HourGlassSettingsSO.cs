using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "Hourglass/Settings", fileName = "Hourglass Settings")]
public class HourGlassSettingsSO : ScriptableObject
{
    [Header("Hourglass Rotation")]
    [Min(0)] public float RotationFailSafeTimer = 5f; // Not An Upgrade
    [Min(0)] public float RotationSpeed = 5;
    public bool AutomaticRotationUnlocked = true;
    [Min(0)] public float AutomaticRotationTime = 2f;
    public bool SpawnSandOnRotate = true;

    [Header("Normal Flow")]
    [Min(0)] public int BallsToFlowThrough = 5;
    [Min(0)] public float BallFlowInterval = 0.2f;
    [Min(0)] public float FlowCheckInterval = 2f;

    [Header("Nudge")]
    [Min(0)] public int BallsNudgeLetThrough = 1;
    [Min(0)] public float NudgeForce = 1;
}
