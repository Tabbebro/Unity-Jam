using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using ScrutableObjects;

public class Timeglass : MonoBehaviour
{
    public static Timeglass Instance;

    [Header("Settings")]
    [ShowProperties] public TimeglassSettingsSO Settings;

    [Header("Rotation Values")]
    public TimeglassRotationValues Rotation;

    [Header("Flow Values")]
    public TimeglassFlowValues Flow;

    [Header("References")]
    [SerializeField] MouseClickNudge _nudge;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }

        // Makes An Copy Of Settings That Can Be Freely Modified Without Changing The Original Values
        Settings = Instantiate(Settings);

        FlowOnAwake();
    }

    private void Update() {
        UpdateFlow();
    }

    private void OnDestroy() {
        FlowOnDestroy();
    }

    #region Flow

    void FlowOnAwake() {
        _nudge.OnSandNudged += Nudge;
    }

    void FlowOnDestroy() {
        _nudge.OnSandNudged -= Nudge;
    }

    void UpdateFlow() {
        if (!Flow.CanFlow) { return; }
        if (Flow.SandFlowRoutine != null) { return; }
        if (Flow.AllHasPassed) { return; }

        Flow.Timer += Time.deltaTime;

        if (Flow.Timer < Settings.FlowCheckInterval && !Flow.FloodGatesOpen) { return; }
        if (Flow.PossibleSand.Count == 0) { return; }

        Flow.Timer = 0;
        Flow.SandFlowRoutine = StartCoroutine(SandFlow());
    }

    public void OnFlowTriggerEnter(Collider2D other) {
        if ((Flow.SandLayer & (1 << other.gameObject.layer)) == 0) { return; }
        if (Flow.PossibleSand.Contains(other.gameObject)) { return; }

        Flow.PossibleSand.Add(other.gameObject);
    }

    public void OnFlowTriggerExit(Collider2D other) {
        if ((Flow.SandLayer & (1 << other.gameObject.layer)) == 0) { return; }

        if (Flow.PossibleSand.Contains(other.gameObject)) { Flow.PossibleSand.Remove(other.gameObject); }
    }

    IEnumerator SandFlow() {
        if (Flow.PossibleSand.Count == 0) { 
            Flow.SandFlowRoutine = null; 
            yield break; 
        }

        Flow.SelectedSand = SelectSand(Settings.SandFlowAmount);
        if (Flow.SelectedSand == null || Flow.SelectedSand.Count == 0) {
            Flow.SandFlowRoutine = null;
            yield break;
        }

        yield return ExecuteFlow(Flow.SelectedSand);

        Flow.SelectedSand.Clear();
        Flow.SandFlowRoutine = null;
    }

    List<GameObject> SelectSand(int amount) {
        // Remove Null Sand, s = Sand
        Flow.PossibleSand.RemoveAll(s => s == null);

        int count = Mathf.Min(amount, Flow.PossibleSand.Count);
        List<GameObject> result = new();
        int safety = 0;

        while (result.Count < count && Flow.PossibleSand.Count > 0 && safety < count) { 
            safety++;

            var sand = Flow.PossibleSand[UnityEngine.Random.Range(0, Flow.PossibleSand.Count)];
            Flow.PossibleSand.Remove(sand);

            if (Flow.UsedSand.Contains(sand)) { continue; }
            if (Flow.SelectedSand.Contains(sand)) { continue; }
            if (Flow.NudgedSand.Contains(sand)) { continue; }

            Flow.UsedSand.Add(sand);
            result.Add(sand);
        }

        return result;
    }

    IEnumerator ExecuteFlow(List<GameObject> sandList) {
        foreach (var sand in sandList) {
            if (sand == null) { continue; }

            Transform target = Rotation.IsRightSideUp ? Flow.BottomPoint : Flow.TopPoint;

            sand.transform.position = target.position;

            PlaySandAudio();

            if (sand.TryGetComponent<BlueGrain>(out _)) {
                if (Flow.FloodRoutine != null) {
                    StopCoroutine(Flow.FloodRoutine);
                }
                Flow.FloodRoutine = StartCoroutine(OpenFloodGates());
            }

            float wait = Flow.FloodGatesOpen ? Settings.FloodFlowDelay : Settings.SandFlowDelay;

            Flow.InvokeOnSandPassed(sand);
            yield return new WaitForSeconds(wait);
        }
    }

    IEnumerator OpenFloodGates() {
        Flow.FloodGatesOpen = true;
        yield return new WaitForSeconds(0.5f);
        Flow.FloodGatesOpen = false;
    }

    public void Nudge() {
        if (!Flow.CanFlow) { return; }
        if (Flow.SandFlowRoutine != null) { return; }

        StartCoroutine(NudgedFlow());
    }

    IEnumerator NudgedFlow() {
        var nudgedSand = SelectSand(Settings.NudgeLetThroughAmount);

        if (nudgedSand == null || nudgedSand.Count == 0) { yield break; }

        Flow.NudgedSand = nudgedSand;

        yield return ExecuteFlow(Flow.NudgedSand);

        Flow.NudgedSand.Clear();
    }

    public void ResetFlow() {
        Flow.PossibleSand.Clear();
        Flow.SelectedSand.Clear();
        Flow.NudgedSand.Clear();
        Flow.UsedSand.Clear();

        Flow.AllHasPassed = false;
        Flow.Timer = 0;

        if (Flow.SandFlowRoutine != null) {
            StopCoroutine(Flow.SandFlowRoutine);
            Flow.SandFlowRoutine = null;
        }
    }

    void PlaySandAudio() {
        float randomVolume = UnityEngine.Random.Range(0.5f, 1f);
        AudioClip randomClip = Flow.SandAudioClips[UnityEngine.Random.Range(0, Flow.SandAudioClips.Count)];
        Flow.SandAudioSource.PlayOneShot(randomClip, randomVolume);
    }

    #endregion
}

[System.Serializable]
public class TimeglassRotationValues {
    [SerializeField] public bool IsRightSideUp = true;
}

[System.Serializable]
public class TimeglassFlowValues {
    [Header("Flow Points")]
    public Transform TopPoint;
    public Transform BottomPoint;

    [Header("Audio")]
    public List<AudioClip> SandAudioClips;
    public AudioSource SandAudioSource;

    [Header("Layer Mask")]
    public LayerMask SandLayer;

    [Header("Checks")]
    public bool FloodGatesOpen = false;
    public bool CanFlow = true;
    [ReadOnly] public bool AllHasPassed = false;
    [ReadOnly] public float Timer = 0f;

    [Header("Lists")]
    public List<GameObject> PossibleSand = new();
    public List<GameObject> SelectedSand = new();
    public List<GameObject> NudgedSand = new();
    public HashSet<GameObject> UsedSand = new();

    public Coroutine FloodRoutine;
    public Coroutine SandFlowRoutine;

    public event Action<GameObject> OnSandPassed;

    public void InvokeOnSandPassed(GameObject passedSand) {
        OnSandPassed?.Invoke(passedSand);
    }
}
