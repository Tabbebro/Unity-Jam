using UnityEngine;
using UnityEngine.InputSystem;

public class MouseClickNudge : MonoBehaviour
{
    [Header("Nudge Settings")]
    public float radius = 2f;       // how far to search for rigidbodies
    public float forceAmount = 10f; // how strong the force is
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame) // new Input System
        {
            print("click");
            ClickNudge();
        }
    }
    void ClickNudge()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        // If using legacy Input: Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Finds 2D colliders in a circle
        Collider2D[] colliders = Physics2D.OverlapCircleAll(mouseWorldPos, radius);
        if (colliders.Length > 0) CameraShake.Instance.Shake(0.1f, 1f);

        foreach (Collider2D col in colliders)
        {
            print("found collider");
            Rigidbody2D rb = col.attachedRigidbody;
            if (rb != null)
            {
                print("found rb");
                Vector2 direction = (rb.position - mouseWorldPos).normalized;
                rb.AddForce(direction * forceAmount, ForceMode2D.Impulse);
            }
        }
    }

    // OPTIONAL: draw the search radius in the Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()), radius);
    }
}
