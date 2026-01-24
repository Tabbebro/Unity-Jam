using System.Collections;
using System;
using UnityEngine;
using ScrutableObjects;

public class HourglassLogic : MonoBehaviour
{
    public bool Rotate = false;

    public HourglassState State;

    public event Action<double, FlowDirection> OnSandFlowed;
    public event Action<FlowDirection> OnRotated;

    void Update() {
        if (Rotate) {
            Rotate = false;
            RotateHourglass();
        }

        Tick(Time.deltaTime);

        CheckAutoRotation();
    }

    public void Tick(float deltaTime) {
        if (!State.IsFlowing || State.IsFlowPaused) { return; }

        double maxTransfer = State.Settings.FlowPerSecond * deltaTime;

        if (maxTransfer <= 0) { return; }

        if (State.Direction == FlowDirection.TopToBottom) {
            double moved = Math.Min(State.SandTop, maxTransfer);
            if (moved <= 0) { return; }

            State.SandTop -= moved;
            State.SandBottom += moved;

            OnSandFlowed?.Invoke(moved, State.Direction);
        }
        else {
            double moved = Math.Min(State.SandBottom, maxTransfer);
            if (moved <= 0) { return; }

            State.SandBottom -= moved;
            State.SandTop += moved;

            OnSandFlowed?.Invoke(moved, State.Direction);
        }
    }

    #region Rotation

    public void RotateHourglass() {
        if (!State.CanRotate) { return; }

        // Pause Flow For The Duration Of Rotation
        StartCoroutine(PauseFlow(State.Settings.RotationDuration));

        State.Direction = State.Direction == FlowDirection.TopToBottom 
            ? FlowDirection.BottomToTop 
            : FlowDirection.TopToBottom;

        OnRotated?.Invoke(State.Direction);
    }

    void CheckAutoRotation() {
        if (!State.CanRotate || !State.Settings.AutoRotationUnlocked) { return; }

        State.AutoRotateTimer += Time.deltaTime;

        if (State.AutoRotateTimer >= State.Settings.AutoRotateInterval) {
            State.AutoRotateTimer = 0;
            RotateHourglass();
        }
    }

    #endregion

    #region Add Sand

    public void AddSand(double amount) {
        if (State.Direction == FlowDirection.TopToBottom) {
            AddSandToTop(amount);
        }
        else {
            AddSandToBottom(amount);
        }
    }

    void AddSandToTop(double amount) {
        State.SandTop += amount;
    }

    void AddSandToBottom(double amount) {
        State.SandBottom += amount;
    }

    #endregion

    #region Helper Functions

    IEnumerator PauseFlow(float duration) {
        State.IsFlowPaused = true;
        yield return new WaitForSeconds(duration);
        State.IsFlowPaused = false;
    }

    #endregion
}

[System.Serializable]
public class HourglassState {
    [ShowProperties] public HourglassSettings Settings;

    [Header("Sand Values")]
    public double SandTop;
    public double SandBottom;

    [Header("Flow Direction")]
    public FlowDirection Direction;

    [Header("Flags")]
    // Sand Flags
    [ReadOnly] public double TotalSand => SandTop + SandBottom;

    // Flowing Flags
    [ReadOnly] public bool IsFlowing => Settings.FlowPerSecond > 0;
    [ReadOnly] public bool IsFlowPaused = false;

    // Rotation Flags
    [ReadOnly] public bool CanRotate {
        get {
            if(Direction == FlowDirection.TopToBottom && SandTop > 0){
                return false;
            }
            else if (Direction == FlowDirection.BottomToTop && SandBottom > 0) {
                return false;
            }
            else {
                return true;
            }
        }
    }
    [ReadOnly] public float AutoRotateTimer;
}
