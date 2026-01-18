using UnityEngine;

[CreateAssetMenu(menuName = "Timeglass/Settings", fileName = "Timeglass Settings")]
public class TimeglassSettingsSO : ScriptableObject
{
    [Header("Hourglass Rotation")]
    [Min(0)] public float RotFailSafeTimer = 5f; // Not An Upgrade
    [Min(0)] public float RotSpeed = 5;
    public bool IsAutoRotUnlocked = true;
    [Min(0)] public float AutoRotTime = 2f;
    public int SandSpawnedPerRot = 0;

    [Header("Normal Flow")]
    [Min(0)] public float FlowCheckInterval = 2f;
    [Min(0)] public int SandFlowAmount = 5;
    [Min(0)] public float SandFlowDelay = 0.2f;
    [Min(0)] public float FloodFlowDelay = 0.001f;

    [Header("Nudge")]
    [Min(0)] public int NudgeLetThroughAmount = 1;
    [Min(0)] public float NudgeForce = 1;
}
