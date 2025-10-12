using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Cannon : MonoBehaviour
{
    private Collider spawnArea;

    public GameObject[] fruitPrefabs;
    public GameObject bombPrefab;
    [Range(0f, 1f)]
    public float bombChance = 0.05f;

    public float minSpawnDelay = 0.25f;
    public float maxSpawnDelay = 1f;

    public float minAngle = -15f;
    public float maxAngle = 15f;

    public float minForce = 18f;
    public float maxForce = 22f;

    public float maxLifetime = 5f;

    private void Awake()
    {
        spawnArea = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        StartCoroutine(Spawn());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(2f);

        while (enabled)
        {
            if (fruitPrefabs == null || fruitPrefabs.Length == 0)
            {
                Debug.LogError("FruitPrefabs array is empty!");
                yield return new WaitForSeconds(1f);
                continue;
            }

            GameObject prefab = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];

            if (Random.value < bombChance && bombPrefab != null)
            {
                prefab = bombPrefab;
            }

            Vector3 position = new Vector3
            {
                x = Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
                y = Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y),
                z = Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z)
            };

            Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(minAngle, maxAngle));

            GameObject fruit = Instantiate(prefab, position, rotation);
            Destroy(fruit, maxLifetime);

            // Vérification et application de la force
            Rigidbody rb = fruit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                float force = Random.Range(minForce, maxForce);
                // Utiliser la direction forward du canon au lieu de up du fruit
                Vector3 forceDirection = transform.forward; // ou transform.up selon l'orientation de votre canon
                rb.AddForce(forceDirection * force, ForceMode.Impulse);

                Debug.Log($"Force appliquée: {force} dans la direction: {forceDirection}");
            }
            else
            {
                Debug.LogError($"Aucun Rigidbody trouvé sur {fruit.name}");
            }

            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
        }
    }
}