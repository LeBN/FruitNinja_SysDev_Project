using UnityEngine;
using System.Collections;

public class FruitSpawnerManager : MonoBehaviour
{
    [Header("Spawners")]
    public Transform[] spawners;

    [Header("Objets a lancer")]
    public GameObject[] fruitPrefabs;
    public GameObject bombPrefab;
    public GameObject goldenFruitPrefab;


    [Header("Parametres de spawn")]
    [Range(0f, 1f)]
    public float bombSpawnChance = 0.15f;
    public float spawnInterval = 1.5f;
    public float launchForce = 4f;
    public float minRandomDelay = 0.3f;

    private bool spawningActive = false;

    public bool enableBombs = true;
    public int maxObjectsPerSpawn = 3;


    private IEnumerator SpawnRoutine()
    {
        while (spawningEnabled)
        {
            // Nombre d’objets à spawn par vague (1 à maxObjectsPerSpawn)
            int fruitsToSpawn = Random.Range(1, maxObjectsPerSpawn + 1);

            for (int i = 0; i < fruitsToSpawn; i++)
            {
                SpawnRandomObject();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }


    private void SpawnRandomObject()
    {
        if (spawners.Length == 0) return;

        Transform spawner = spawners[Random.Range(0, spawners.Length)];

        float bombChance = bombSpawnChance;
        float goldenChance = 0.10f; // 10 % de chance d’avoir un fruit doré

        GameObject prefabToSpawn = null;

        float rand = Random.value;

        // Bombe (si activée)
        if (enableBombs && bombPrefab != null && rand < bombChance)
        {
            prefabToSpawn = bombPrefab;
        }
        // Fruit doré
        else if (goldenFruitPrefab != null && rand < bombChance + goldenChance)
        {
            prefabToSpawn = goldenFruitPrefab;
        }
        // Fruit normal
        else if (fruitPrefabs.Length > 0)
        {
            prefabToSpawn = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];
        }

        if (prefabToSpawn == null) return;

        Vector3 spawnPos = spawner.position;
        spawnPos.y += 0.5f;

        GameObject obj = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        Destroy(obj, 10f); // auto-destruction après 10 secondes

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 launchDir = Vector3.up + new Vector3(
                Random.Range(-0.1f, 0.1f),
                0,
                Random.Range(-0.1f, 0.1f)
            );

            rb.AddForce(launchDir.normalized * launchForce, ForceMode.VelocityChange);
            rb.AddTorque(Random.insideUnitSphere * 3f, ForceMode.VelocityChange);
        }

        Debug.Log("Spawned object: " + obj.name);
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
