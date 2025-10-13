using UnityEngine;

public class FruitRotation : MonoBehaviour
{
    [Header("Vitesse de rotation")]
    public float rotationSpeed = 180f; // degrés/seconde

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Si un Rigidbody existe, on lui applique une rotation aléatoire initiale
        if (rb != null)
        {
            Vector3 randomTorque = Random.insideUnitSphere * (rotationSpeed * 0.1f);
            rb.AddTorque(randomTorque, ForceMode.VelocityChange);
        }
    }

    void Update()
    {
        // Si pas de Rigidbody, on fait tourner manuellement
        if (rb == null)
        {
            Vector3 randomAxis = new Vector3(
                Mathf.Sin(Time.time * 1.3f),
                1f,
                Mathf.Cos(Time.time * 0.8f)
            ).normalized;

            transform.Rotate(randomAxis * rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}
