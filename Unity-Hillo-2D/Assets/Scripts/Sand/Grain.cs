using Unity.VisualScripting;
using UnityEngine;

public class Grain : MonoBehaviour
{
    public bool hasGoneThrough;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("DeathArea"))
        {
            transform.position = Vector2.up * 5;
        }
    }
}
