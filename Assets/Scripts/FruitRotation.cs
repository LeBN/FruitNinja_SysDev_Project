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
            Vector3 randomTorque = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)
            ) * rotationSpeed;

            rb.AddTorque(randomTorque, ForceMode.VelocityChange);
        }
    }

    void Update()
    {
        // Si pas de Rigidbody, on fait tourner manuellement
        if (rb == null)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}
