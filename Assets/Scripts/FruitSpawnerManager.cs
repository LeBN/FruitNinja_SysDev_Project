using UnityEngine;
using System.Collections;

public class FruitSpawnerManager : MonoBehaviour
{
    [Header("Spawners")]
    public Transform[] spawners;

    [Header("Objets a lancer")]
    public GameObject[] fruitPrefabs;
    public GameObject bombPrefab;

    [Header("Parametres de spawn")]
    [Range(0f, 1f)]
    public float bombSpawnChance = 0.15f;
    public float spawnInterval = 1.5f;
    public float launchForce = 4f;
    public float minRandomDelay = 0.3f;

    private bool spawningActive = false;

    private IEnumerator SpawnRoutine()
    {
        while (spawningActive)
        {
            yield return new WaitForSeconds(spawnInterval + Random.Range(-minRandomDelay, minRandomDelay));

            // Choisit combien d’objets spawnent cette fois (1 à 3)
            int objectsToSpawn = Random.Range(1, 4); // 1, 2 ou 3
            for (int i = 0; i < objectsToSpawn; i++)
            {
                SpawnRandomObject();
                yield return new WaitForSeconds(0.2f); // petit délai entre les spawns d’un même groupe
            }
        }
    }

    private void SpawnRandomObject()
    {
        if (spawners.Length == 0) return;

        Transform spawner = spawners[Random.Range(0, spawners.Length)];
        GameObject prefabToSpawn = null;

        // Chance de bombe
        if (bombPrefab != null && Random.value < bombSpawnChance)
            prefabToSpawn = bombPrefab;
        else if (fruitPrefabs.Length > 0)
            prefabToSpawn = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];

        if (prefabToSpawn == null) return;

        Vector3 spawnPos = spawner.position;
        spawnPos.y += 0.5f;

        GameObject obj = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        // Auto-destruction apres 10 secondes
        Destroy(obj, 1.5f);

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Direction verticale avec tres legere deviation
            Vector3 launchDir = Vector3.up + new Vector3(Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f));
            rb.AddForce(launchDir.normalized * launchForce, ForceMode.VelocityChange);
            rb.AddTorque(Random.insideUnitSphere * 3f, ForceMode.VelocityChange);
        }
    }

    public void StartSpawning()
    {
        if (!spawningActive)
        {
            spawningActive = true;
            StartCoroutine(SpawnRoutine());
        }
    }

    public void StopSpawning()
    {
        spawningActive = false;
        StopAllCoroutines();
    }
}
