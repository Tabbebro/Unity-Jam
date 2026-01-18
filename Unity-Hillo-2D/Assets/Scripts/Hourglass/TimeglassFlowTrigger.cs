using UnityEngine;

public class TimeglassFlowTrigger : MonoBehaviour
{
    public Timeglass Parent;

    private void Awake() {
        if (Parent == null) {
            Parent = GetComponentInParent<Timeglass>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Parent.OnFlowTriggerEnter(collision);
    }

    private void OnTriggerExit2D(Collider2D collision) {
        Parent.OnFlowTriggerExit(collision);
    }
}
