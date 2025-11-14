using NUnit.Framework;
using System.Collections;
using System.Numerics;
using UnityEngine;

public class AssignRigidBodies : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    //
    public GameObject[] static_world_objects;
    public GameObject[] planks_to_fall;

    void Start()
    {
        // give a rigidbody and mesh collider to each "static" world object
        // these shouldn't actually be static because unity combines static objects into a single mesh,
        // which is not what we want when handling convex collision
        foreach (GameObject obj in static_world_objects) {
            if (obj == null) continue;

            MeshCollider mesh_collider = obj.AddComponent<MeshCollider>();
            mesh_collider.convex = true;

            Rigidbody rigid_body = obj.AddComponent<Rigidbody>();
            rigid_body.isKinematic = true; // set to static
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void drop_planks()
    {
        StartCoroutine(drop_planks_coroutine());
    }

    public IEnumerator drop_planks_coroutine()
    {
        foreach (GameObject obj in planks_to_fall) {
            if (obj == null) continue;

            Rigidbody rigid_body = obj.GetComponent<Rigidbody>();
            rigid_body.isKinematic = false; // set to false for it to fall

            // wait for 0.2 seconds
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(0);
    }
}
