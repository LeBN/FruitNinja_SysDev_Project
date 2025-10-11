using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EzySlice;
using UnityEngine.InputSystem;


public class SlideObject : MonoBehaviour
{
    public Transform startSlicePoint;
    public Transform endSlicePoint;
    public LayerMask sliceableLayer;
    public VelocityEstimator velocityEstimator;

    [SerializeField] public Material crossSectionMaterial;
    public float cutForce = 2000;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Correction : FixedUpdate doit commencer par une majuscule
    void FixedUpdate()
    {
        bool hasHit = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, sliceableLayer);
        if (hasHit)
        {
            GameObject target = hit.collider.gameObject;
            SliceObject(target);
        }
    }

    public void SliceObject(GameObject target)
    {
        Vector3 planePoint = target.transform.position;
        Vector3 planeNormal = Vector3.up;

        SlicedHull hull = target.Slice(planePoint, planeNormal, crossSectionMaterial);

        if (hull == null)
        {
            Debug.Log("EzySlice ne parvient pas à découper " + target.name);
            return;
        }

        GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterial);
        GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterial);

        AddPhysics(upperHull);
        AddPhysics(lowerHull);

        Destroy(target);
    }

    private void AddPhysics(GameObject slicedObject)
    {
        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        collider.convex = true;
        rb.AddExplosionForce(cutForce, slicedObject.transform.position, 5f);
    }

}
