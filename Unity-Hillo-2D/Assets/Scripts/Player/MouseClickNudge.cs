using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseClickNudge : MonoBehaviour
{
    [Header("Nudge Settings")]
    public float radius = 2f;       
    public float forceAmount = 10f; 
    public float cooldown = 1f;
    float timer;
    public bool canNudge;
    public event Action OnSandNudged;
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && canNudge) 
        {
            //print("click");
            ClickNudge();
            canNudge = false;
        }
        else if (!canNudge)
        {
            timer += Time.deltaTime;
            if (timer >= cooldown) 
            {
                timer = 0;
                canNudge = true;
            }
        }
    }
    void ClickNudge()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        Collider2D[] colliders = Physics2D.OverlapCircleAll(mouseWorldPos, radius);


        // if the collider is higher than y 0
        if (colliders.Length > 0 && colliders[0].transform.position.y > -0.5f)
        {
            OnSandNudged?.Invoke();
            CameraShake.Instance.Shake(0.1f, 1f);
            
            foreach (Collider2D col in colliders)
            {
                //print("found collider");
                Rigidbody2D rb = col.attachedRigidbody;
                if (rb != null)
                {
                    //print("found rb");
                    Vector2 direction = (rb.position - mouseWorldPos).normalized;
                    rb.AddForce(direction * forceAmount, ForceMode2D.Impulse);
                }
            }
            
        } 
    }
}
