using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpeedBooster : MonoBehaviour
{
    public float forceAmount = 10f; // Positive value for force magnitude

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            rb.AddForce(Vector3.back * forceAmount, ForceMode.Impulse);
        }
    }
}
