using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockTrigger : MonoBehaviour
{

    public Rigidbody[] rigidbodies;
    public ParticleSystem dust_particle_system;
    public ParticleSystem water_particle_system;
    public AudioSource rocks_falling_audio;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        // step 5 goes to 6
        if (other.CompareTag("VRHead"))
        {
            dust_particle_system.Play();
            water_particle_system.Play();
            rocks_falling_audio.Play();
            // set rocks to fall
            foreach (Rigidbody rigidbody in rigidbodies)
            {
                rigidbody.isKinematic = false;
            }
        }
    }

}
