using UnityEngine;

[CreateAssetMenu(menuName = "Hourglass/Physics Settings", fileName = "Timeglass Settings")]
public class TimeglassSettingsSO : ScriptableObject
{
    [Header("Hourglass Rotation")]
    [Min(0)] public float RotationFailSafeTimer = 5f; // Not An Upgrade
    [Min(0)] public float RotationSpeed = 5;
    public bool IsAutoRotationUnlocked = true;
    [Min(0)] public float AutoRotationTime = 2f;
    public int SandSpawnedPerRotation = 0;

    [Header("Normal Flow")]
    [Min(0)] public float FlowCheckInterval = 2f;
    [Min(0)] public int SandFlowAmount = 5;
    [Min(0)] public float SandFlowDelay = 0.2f;
    [Min(0)] public float FloodFlowDelay = 0.001f;

    [Header("Nudge")]
    [Min(0)] public int NudgeLetThroughAmount = 1;
    [Min(0)] public float NudgeForce = 1;

    [Header("Sand Settings")]
    [Range(0, 1)] public float SandSpawnDelay = 0.1f;
    [Min(0)] public int SandNormalMaxAmount = 100;
}
