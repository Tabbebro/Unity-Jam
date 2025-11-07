using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private Vector3 originalPos;  
    private float noiseSeed;

    private void Awake()
    {
        Instance = this;
    }

    public void Shake(float duration = 0.1f, float magnitude = 0.1f)
    {
        StopAllCoroutines();
        StartCoroutine(DoShake(duration, magnitude));
    }

    private IEnumerator DoShake(float duration, float magnitude)
    {
        noiseSeed = Random.value * 100f;
        originalPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Perlin noise gives smooth values over time
            float x = (Mathf.PerlinNoise(elapsed * 10f, noiseSeed) - 0.5f) * 2f * magnitude;
            float y = (Mathf.PerlinNoise(noiseSeed, elapsed * 10f) - 0.5f) * 2f * magnitude;

            transform.localPosition = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
