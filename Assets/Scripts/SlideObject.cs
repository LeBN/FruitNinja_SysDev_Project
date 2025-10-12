using UnityEngine;
using EzySlice;
using System.Collections;
using System.Collections.Generic;

public class SlideObject : MonoBehaviour
{
    public Transform startSlicePoint;
    public Transform endSlicePoint;
    public LayerMask sliceableLayer;
    public Material crossSectionMaterial;
    public float cutForce = 2000f;

    public GameObject explosionVFX;
    public AudioClip fruitCutSound;
    public AudioClip bombExplosionSound;

    public string bombTag = "Bomb";
    public string startButtonTag = "StartButton";
    public GameObject gameManager;

    private AudioSource audioSource;

    // Liste des objets deja coupes recemment
    private HashSet<GameObject> recentlySliced = new HashSet<GameObject>();
    private float sliceCooldown = 0.15f; // 150 ms d'intervalle de securite

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        if (gameManager == null)
        {
            GameManager gmSearch = GameObject.FindObjectOfType<GameManager>();
            if (gmSearch != null)
                gameManager = gmSearch.gameObject;
        }
    }

    void FixedUpdate()
    {
        if (Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, sliceableLayer))
        {
            GameObject target = hit.collider.gameObject;
            if (!recentlySliced.Contains(target))
            {
                StartCoroutine(RegisterSliceCooldown(target)); // Marque cet objet comme "deja coupe" pendant un court delai
                SliceObject(target);
            }
        }
    }

    private IEnumerator RegisterSliceCooldown(GameObject obj)
    {
        recentlySliced.Add(obj);
        yield return new WaitForSeconds(sliceCooldown);
        recentlySliced.Remove(obj);
    }

    public void SliceObject(GameObject target)
    {
        if (target == null) return;

        GameManager gm = gameManager != null ? gameManager.GetComponent<GameManager>() : null;

        // Fruit de demarrage
        if (target.CompareTag(startButtonTag))
        {
            if (gm != null)
                gm.StartGame();

            if (explosionVFX != null)
                Instantiate(explosionVFX, target.transform.position, Quaternion.identity);

            PlaySound(fruitCutSound);
            Destroy(target);
            return;
        }

        // Bombe
        if (target.CompareTag(bombTag))
        {
            PlaySound(bombExplosionSound);

            if (explosionVFX != null)
                Instantiate(explosionVFX, target.transform.position, Quaternion.identity);

            if (gm != null)
            {
                gm.AddScore(-3);
                gm.ResetFruitsOnBomb();
            }

            Destroy(target);
            return;
        }

        // Fruit normal
        SlicedHull hull = target.Slice(target.transform.position, Vector3.up, crossSectionMaterial);
        if (hull == null) return;

        GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterial);
        GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterial);

        Rigidbody originalRb = target.GetComponent<Rigidbody>();
        Vector3 originalVelocity = originalRb ? originalRb.linearVelocity : Vector3.zero;
        Vector3 originalAngularVelocity = originalRb ? originalRb.angularVelocity : Vector3.zero;

        AddPhysicsToSlice(upperHull, originalVelocity, originalAngularVelocity);
        AddPhysicsToSlice(lowerHull, originalVelocity, originalAngularVelocity);

        PlaySound(fruitCutSound);

        if (gm != null)
            gm.AddScore(1);

        Destroy(target);
    }

    private void AddPhysicsToSlice(GameObject slicedObject, Vector3 inheritedVelocity, Vector3 inheritedAngularVelocity)
    {
        if (slicedObject == null) return;

        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        SphereCollider sc = slicedObject.AddComponent<SphereCollider>();

        rb.linearVelocity = inheritedVelocity;
        rb.angularVelocity = inheritedAngularVelocity;
        rb.AddExplosionForce(cutForce, slicedObject.transform.position, 5f);
        rb.AddTorque(Random.insideUnitSphere * 3f, ForceMode.Impulse);
        Destroy(slicedObject, 8f);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }
}
