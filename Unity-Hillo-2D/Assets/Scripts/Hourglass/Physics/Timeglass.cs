using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using ScrutableObjects;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Pool;
using UnityEngine.InputSystem;
using System.Linq;

public class Timeglass : MonoBehaviour
{
    public static Timeglass Instance;

    [Header("Settings")]
    [ShowProperties] public TimeglassSettingsSO Settings;

    [Header("Rotation Values")]
    public TimeglassRotationValues Rotation;

    [Header("Flow Values")]
    public TimeglassFlowValues Flow;

    [Header("Sand Values")]
    public TimeglassSandValues Sand;

    [Header("Nudge Values")]
    public TimeglassNudgeValues Nudge;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }

        // Makes An Copy Of Settings That Can Be Freely Modified Without Changing The Original Values
        Settings = Instantiate(Settings);

        RotationOnAwake();
        FlowOnAwake();
        SandOnAwake();
    }

    private void Update() {
        FlowOnUpdate();
        RotationOnUpdate();
        SandOnUpdate();
        NudgeOnUpdate();
    }

    private void OnDestroy() {
        RotationOnDestroy();
        FlowOnDestroy();
        SandOnDestroy();
    }

    #region Rotation

    void RotationOnAwake() {
        Rotation.OnRotationStarted += PlayRotateAudio;
        Rotation.OnRotationStarted += DisableRotationButton;
        Rotation.OnRotationStarted += ResetAutoRotation;
        Rotation.OnCanRotate += OnCanRotate;

    }

    void RotationOnDestroy() {
        Rotation.OnRotationStarted -= PlayRotateAudio;
        Rotation.OnRotationStarted -= DisableRotationButton;
        Rotation.OnRotationStarted -= ResetAutoRotation;
        Rotation.OnCanRotate -= OnCanRotate;

    }

    void RotationOnUpdate() {
        if (!Flow.CanFlow) { return; }
        if (Rotation.IsRotating) { return; }
        AutoRotate();
    }

    public void RotateTimeglass() {
        if (Rotation.IsRotating) { return; }
        KillRotationTween();

        // Flip bool around
        Rotation.IsRightSideUp = !Rotation.IsRightSideUp;
        Rotation.IsRotating = true;
        Rotation.InvokeOnRotationStarted();

        float targetRotation = Rotation.VisualRB.rotation + 180;
        DOTween.To(() => Rotation.VisualRB.rotation, x => Rotation.VisualRB.MoveRotation(x), targetRotation, Settings.RotationSpeed).SetEase(Ease.InOutQuad).OnComplete(FinishTimeglassRotation);
    }

    void FinishTimeglassRotation() {
        Rotation.IsRotating = false;
        Rotation.InvokeOnRotationFinished();
    }

    void AutoRotate() {
        if (!Settings.IsAutoRotationUnlocked || !Rotation.CanRotate || Rotation.IsRotating) { return; }
        Rotation.Timer += Time.deltaTime;

        if (!Rotation.AutoRotationTimerMask.activeInHierarchy) { 
            Rotation.AutoRotationTimerMask.SetActive(true); 
        }

        Rotation.AutoRotationTimerFill.fillAmount = Rotation.Timer / Settings.AutoRotationTime;

        if (Rotation.Timer >= Settings.AutoRotationTime) {
            RotateTimeglass();
        }
    }

    void ResetAutoRotation() {
        Rotation.Timer = 0;
        Rotation.AutoRotationTimerFill.fillAmount = 0;
        Rotation.AutoRotationTimerMask.SetActive(false);
    }

    void OnCanRotate(bool status) {
        
        // Set Can Rotate Status
        Rotation.CanRotate = status;
        
        // Set Button Status
        if (status) {
            EnableRotationButton();
        }
        else {
            DisableRotationButton();
        }
    }

    #region Rotation Tweens

    void KillRotationTween() {
        if (Rotation.RotationTween != null && Rotation.RotationTween.IsActive()) {
            Rotation.RotationTween.Kill();
            Rotation.RotationTween = null;
        }
    }

    #endregion

    #region Rotation Button

    public void EnableRotationButton() {
        Rotation.CanRotate = true;
        Rotation.RotationButton.interactable = true;
    }

    public void DisableRotationButton() {
        Rotation.CanRotate = false;
        Rotation.RotationButton.interactable = false;
    }

    public void PlayRotateAudio() {
        Rotation.AudioSource.pitch = Rotation.AudioClip.length / Settings.RotationSpeed;
        Rotation.AudioSource.PlayOneShot(Rotation.AudioClip);
    }

    #endregion

    #endregion

    #region Flow

    void FlowOnAwake() {
        Nudge.OnSandNudged += SandNudged;
        Rotation.OnRotationStarted += ResetFlow;
        Rotation.OnRotationStarted += DisableFlow;
        Rotation.OnRotationFinished += EnableFlow;
    }

    void FlowOnDestroy() {
        Nudge.OnSandNudged -= SandNudged;
        Rotation.OnRotationStarted -= ResetFlow;
        Rotation.OnRotationStarted -= DisableFlow;
        Rotation.OnRotationFinished -= EnableFlow;
    }

    void FlowOnUpdate() {
        if (!Flow.CanFlow) { return; }
        if (Flow.SandFlowRoutine != null) { return; }
        if (Flow.AllHasPassed) { return; }

        Flow.Timer += Time.deltaTime;

        if (Flow.Timer < Settings.FlowCheckInterval && !Flow.FloodGatesOpen) { return; }
        if (Flow.PossibleSand.Count == 0) { return; }

        Flow.Timer = 0;
        Flow.SandFlowRoutine = StartCoroutine(SandFlow());
    }

    void EnableFlow() {
        Flow.CanFlow = true;
    }

    void DisableFlow() {
        Flow.CanFlow = false;
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

            // Random Sand
            // var sand = Flow.PossibleSand[UnityEngine.Random.Range(0, Flow.PossibleSand.Count)];

            // First Sand Under Certain Y Level
            var sand = Flow.PossibleSand.FirstOrDefault(s => s.transform.position.y < Flow.YHeight);

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
        foreach (var sand in new List<GameObject>(sandList)) {
            if (sand == null) { continue; }

            Transform target = Rotation.IsRightSideUp ? Flow.BottomPoint : Flow.TopPoint;

            sand.transform.position = target.position;

            PlaySandAudio();

            SandGrain sandGrain;
            if (sand.TryGetComponent<SandGrain>(out sandGrain)) {
                if (sandGrain.Type == SandType.Blue) {
                    if (Flow.FloodRoutine != null) {
                        StopCoroutine(Flow.FloodRoutine);
                    }
                    Flow.FloodRoutine = StartCoroutine(OpenFloodGates());
                }
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

    IEnumerator NudgedFlow() {
        var nudgedSand = SelectSand(Settings.NudgeLetThroughAmount);

        if (nudgedSand == null || nudgedSand.Count == 0) { yield break; }

        Flow.NudgedSand = nudgedSand;

        yield return ExecuteFlow(Flow.NudgedSand);

        Flow.NudgedSand.Clear();
    }

    public void ResetFlow() {

        if (Flow.SandFlowRoutine != null) {
            StopCoroutine(Flow.SandFlowRoutine);
            Flow.SandFlowRoutine = null;
        }

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
        AudioClip randomClip = Flow.AudioClips[UnityEngine.Random.Range(0, Flow.AudioClips.Count)];
        Flow.AudioSource.PlayOneShot(randomClip, randomVolume);
    }

    #endregion

    #region Sand System

    void SandOnAwake() {
        SetPossibleSand();

        Flow.OnSandPassed += TrackSand;
        Rotation.OnRotationFinished += CheckIfAllPassed;
        Rotation.OnRotationFinished += SetPossibleSand;

        SandPoolInit();
    }

    void SandOnDestroy() {

        Flow.OnSandPassed -= TrackSand;
        Rotation.OnRotationFinished -= CheckIfAllPassed;
        Rotation.OnRotationFinished -= SetPossibleSand;
    }

    void SandOnUpdate() {
        if (Sand.DebugSpawnSand) {
            Sand.DebugSpawnSand = false;
            SpawnSand(100);
        }
    }

    void TrackSand(GameObject sand) {
        if (sand == null) {
            Debug.LogError("<color=red> Null Sand Passed </color>");
            return; 
        }

        if (Rotation.IsRightSideUp) {
            Sand.TopSand.Remove(sand);
            Sand.BottomSand.Add(sand);
        }
        else {
            Sand.BottomSand.Remove(sand);
            Sand.TopSand.Add(sand);
        }

        CheckIfAllPassed();
    }

    void CheckIfAllPassed() {
        if (Rotation.IsRightSideUp) {
            Flow.AllHasPassed = Sand.TopSand.Count == 0;
        }
        else {
            Flow.AllHasPassed = Sand.BottomSand.Count == 0;
        }
        Rotation.InvokeOnCanRotate(Flow.AllHasPassed);
    }

    void SetPossibleSand() {
        if (Rotation.IsRightSideUp) {
            Flow.PossibleSand = new List<GameObject>(Sand.TopSand);
        }
        else {
            Flow.PossibleSand = new List<GameObject>(Sand.BottomSand);
        }
        CheckIfAllPassed();
    }

    void SpawnSand(int amount) {

        StartCoroutine(SandSpawnRoutine(amount));
    }

    IEnumerator SandSpawnRoutine(int amount) {

        yield return new WaitUntil(() => Rotation.IsRotating == false);

        for (int i = 0; i < amount; i++) {

            SpawnSingleSand();

            yield return new WaitForSeconds(0.1f);
        }
    }

    void SpawnSingleSand() {
        var entry = GetRandomSandEntry();
        if (entry == null) { return; }

        GameObject sandObject = entry.Pool.Get();
        entry.ActiveSand.Add(sandObject);
    }

    // For Weighted Sand
    SandSpawnEntry GetRandomSandEntry() {
        float totalWeight = 0f;

        // Get Total Weight
        foreach (var entry in Settings.SandTypes) {
            // Don't Count If Max Amount Of Said Sand
            if (entry.ActiveSand.Count >= entry.MaxAmount) { continue; }

            totalWeight += entry.Weight;
        }

        // Get Random Value From Total Weight
        float randomValue = UnityEngine.Random.Range(0f, totalWeight);

        float cumulative = 0f;

        // Get The Sand Using Random Value
        foreach (var entry in Settings.SandTypes) {
            // Don't Count If Max Amount Of Said Sand
            if (entry.ActiveSand.Count >= entry.MaxAmount) { continue; }

            cumulative += entry.Weight;
            if (randomValue <= cumulative) {
                return entry;
            }
        }

        // Fall Back Everything Fails
        return null;
    }

    #region Sand Pooling

    public void SandPoolInit() {
        foreach (var entry in Settings.SandTypes) {
            entry.Pool = new ObjectPool<GameObject>(
                createFunc: () => CreateSand(entry),
                actionOnGet: OnGetSand,
                actionOnRelease: OnReleaseSand,
                actionOnDestroy: OnDestroySand,
                collectionCheck: true,
                defaultCapacity: 10,
                maxSize: Sand.SandPoolMaxCount
                );
        }
    }

    GameObject CreateSand(SandSpawnEntry entry) {
        GameObject sandObject = Instantiate(entry.Prefab, Sand.SandSpawnPoint);
        sandObject.transform.parent = Sand.PoolParent;
        sandObject.transform.localScale = Vector3.one;
        return sandObject;
    }

    void OnGetSand(GameObject gameObject) {

        gameObject.transform.parent = Sand.SandParent;

        if (Rotation.IsRightSideUp) {
            Sand.TopSand.Add(gameObject);
        }
        else {
            Sand.BottomSand.Add(gameObject);
        }
        SetPossibleSand();

        gameObject.SetActive(true);
    }

    void OnReleaseSand(GameObject gameObject) {

        SandGrain grain = gameObject.GetComponent<SandGrain>();

        if (grain != null) {
            var entry = Settings.SandTypes.Find(e => e.Type == grain.Type);
            if (entry != null) {
                entry.ActiveSand.Remove(gameObject);
            }
        }

        if (Sand.TopSand.Contains(gameObject)) {
            Sand.TopSand.Remove(gameObject);
        }
        else if (Sand.BottomSand.Contains(gameObject)) {
            Sand.BottomSand.Remove(gameObject);
        }
        else {
            Debug.LogError("<color=red> Released Sand Is Not In Either Top Or Bottom </color>");
        }

        SetPossibleSand();

        gameObject.transform.parent = Sand.PoolParent;
        gameObject.SetActive(false);
    }

    void OnDestroySand(GameObject gameObject) {
        Destroy(gameObject);
    }

    #endregion

    #endregion

    #region Nudge

    void NudgeOnUpdate() {
        if (Mouse.current.leftButton.wasPressedThisFrame && Nudge.CanNudge) {
            NudgeClick();
            Nudge.CanNudge = false;
        }
        else if (!Nudge.CanNudge) {
            Nudge.Timer += Time.deltaTime;
            if (Nudge.Timer < Settings.NudgeCooldown) { return; }
            Nudge.Timer = 0;
            Nudge.CanNudge = true;
        }
    }

    void NudgeClick() {
        // Check For Nulls
        if (Nudge.Cam == null) { Nudge.Cam = Camera.main; }
        Nudge.CurrentMouse ??= Mouse.current; // This Is Weird

        // Get Location For Nudge
        Nudge.Location = Nudge.Cam.ScreenToWorldPoint(Nudge.CurrentMouse.position.ReadValue());

        // Get Overlapped Grains
        Nudge.Buffer = Physics2D.OverlapCircleAll(Nudge.Location, Settings.NudgeRadius);

        if (Nudge.Buffer.Length > 0 && Nudge.Buffer[0].transform.position.y >= Nudge.YHeight) {
            Nudge.InvokeOnSandNudged();
            CameraShake.Instance.Shake(Nudge.CameraShakeDuration, Nudge.CameraShakeMagnitude);

            foreach (Collider2D col in Nudge.Buffer) {
                Rigidbody2D rb = col.attachedRigidbody;
                if (rb == null) { continue; }

                Vector2 direction = (rb.position - Nudge.Location).normalized;
                rb.AddForce(direction * Settings.NudgeImpulseForceAmount, ForceMode2D.Impulse);
            }
        }
    }

    public void SandNudged() {
        if (!Flow.CanFlow) { return; }
        if (Flow.SandFlowRoutine != null) { return; }

        StartCoroutine(NudgedFlow());
    }

    #endregion

    #region OnGizmos

    private void OnDrawGizmosSelected() {

        float xOffset;
        Vector2 StartPos;
        Vector2 EndPos;

        // Flow Y Check Gizmos
        xOffset = 20f;
        StartPos = new Vector2(transform.position.x - xOffset , Flow.YHeight);
        EndPos = new Vector2(transform.position.x + xOffset , Flow.YHeight);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(StartPos, EndPos);

        // Nudge Y Check Gizmos
        StartPos = new Vector2(transform.position.x - xOffset, Nudge.YHeight);
        EndPos = new Vector2(transform.position.x + xOffset, Nudge.YHeight);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(StartPos, EndPos);
    }

    #endregion

}

#region Rotation Class

[System.Serializable]
public class TimeglassRotationValues {
    [Header("Refs")]
    public Button RotationButton;
    public Rigidbody2D VisualRB;
    public GameObject AutoRotationTimerMask;
    public Image AutoRotationTimerFill;

    [Header("Audio")]
    public AudioClip AudioClip;
    public AudioSource AudioSource;

    [Header("Checks")]
    [ReadOnly] public bool IsRotating = false;
    [ReadOnly] public bool CanRotate = false;
    [ReadOnly] public bool IsRightSideUp = true;

    [Header("Auto Rotation Timer")]
    [ReadOnly] public float Timer;

    // Tweens
    public Tween RotationTween;

    // Events
    public event Action<bool> OnCanRotate;
    public event Action OnRotationStarted;
    public event Action OnRotationFinished;
    
    public void InvokeOnCanRotate(bool status) {
        OnCanRotate?.Invoke(status);
    }

    public void InvokeOnRotationStarted() {
        OnRotationStarted?.Invoke(); 
    }

    public void InvokeOnRotationFinished() {
        OnRotationFinished?.Invoke(); 
    }
}

#endregion

#region Flow Class

[System.Serializable]
public class TimeglassFlowValues {
    [Header("Flow Y Height")]
    public float YHeight;

    [Header("Flow Points")]
    public Transform TopPoint;
    public Transform BottomPoint;

    [Header("Audio")]
    public List<AudioClip> AudioClips;
    public AudioSource AudioSource;

    [Header("Layer Mask")]
    public LayerMask SandLayer;

    [Header("Checks")]
    [ReadOnly] public bool FloodGatesOpen = false;
    [ReadOnly] public bool CanFlow = true;
    [ReadOnly] public bool AllHasPassed = false;
    [ReadOnly] public float Timer = 0f;

    [Header("Lists")]
    [ReadOnly] public List<GameObject> PossibleSand = new();
    [ReadOnly] public List<GameObject> SelectedSand = new();
    [ReadOnly] public List<GameObject> NudgedSand = new();
    [ReadOnly] public HashSet<GameObject> UsedSand = new();

    public Coroutine FloodRoutine;
    public Coroutine SandFlowRoutine;

    public event Action<GameObject> OnSandPassed;

    public void InvokeOnSandPassed(GameObject passedSand) {
        OnSandPassed?.Invoke(passedSand);
    }
}

#endregion

#region Sand Class

[System.Serializable]
public class TimeglassSandValues {
    [Header("DEBUG SPAWN SAND")]
    public bool DebugSpawnSand = false;


    [Header("Sand Spawn Settings")]
    public Transform SandParent;
    public Transform SandSpawnPoint;

    [Header("Sand Pool")]
    public Transform PoolParent;
    public int SandPoolMaxCount = 100;


    [Header("Sand Lists")]
    [ReadOnly] public List<GameObject> TopSand = new();
    [ReadOnly] public List<GameObject> BottomSand = new();
}

#endregion

#region Nudge Class

[System.Serializable]
public class TimeglassNudgeValues {
    [Header("Nudge Flags")]
    [ReadOnly] public bool CanNudge;
    [ReadOnly] public float Timer;

    [Header("Nudge Settings")]
    public float YHeight = -0.5f;
    public float CameraShakeDuration = 0.1f;
    public float CameraShakeMagnitude = 1f;

    // Cache
    [HideInInspector] public Camera Cam;
    [HideInInspector] public Mouse CurrentMouse;
    [HideInInspector] public Collider2D[] Buffer;
    [HideInInspector] public Vector2 Location;
    
    public event Action OnSandNudged;

    public void InvokeOnSandNudged() {
        OnSandNudged?.Invoke();
    }
}

#endregion
