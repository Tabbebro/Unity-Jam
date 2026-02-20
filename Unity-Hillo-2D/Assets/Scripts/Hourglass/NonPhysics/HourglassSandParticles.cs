using UnityEngine;

public class HourglassSandParticles : MonoBehaviour
{
    [Header("Refs")]
    public HourglassLogic Logic;
    public ParticleSystem SandParticles;

    [Header("Tuning")]
    [SerializeField] float _particlesPerSandUnit = 50f;
    [SerializeField] float _maxEmissionRate = 500f;

    ParticleSystem.EmissionModule _emission;

    private void Awake() {
        _emission = SandParticles.emission;
        _emission.rateOverTime = 0;
    }

    void OnEnable() {
        Logic.OnSandFlowed += OnSandFlowed;
        Logic.OnRotated += OnRotated;
    }

    private void OnDisable() {
        Logic.OnSandFlowed -= OnSandFlowed;
        Logic.OnRotated -= OnRotated;
    }

    void OnSandFlowed(double amount, FlowDirection direction) {
        float rate = (float)amount * _particlesPerSandUnit / Time.deltaTime;
        rate = Mathf.Clamp(rate, 0, _maxEmissionRate);

        _emission.rateOverTime = rate;
    }

    void OnRotated(FlowDirection direction) {
        _emission.rateOverTime = 0;
    }
}
