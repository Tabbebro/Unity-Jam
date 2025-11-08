using System;
using Unity.VisualScripting;
using UnityEngine;

public class Grain : MonoBehaviour
{
    public bool hasGoneThrough;
    SandManager sandManager;
    public float worth = 1;
    void Start()
    {
        Hourglass.Instance.OnRotationFinished += HourglassRotated;
        sandManager = transform.parent.GetComponent<SandManager>();
    }
    public void IncreaseWorth(float multiplier = 1)
    {
        worth *= multiplier;
    }

    private void HourglassRotated()
    {
        hasGoneThrough = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("DeathArea"))
        {
            transform.position = Vector2.up * 5;
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BottomArea") && !hasGoneThrough)
        {
            if (sandManager.AllSandGoneThrough)
            {
                sandManager.AllSandGoneThrough = false;
                sandManager.howManyGoneThrough = 0;
            }
            hasGoneThrough = true;
            sandManager.SandWentThrough();
            UpgradeManager.Instance.ModifySandResource(1);
        }
    }
}
