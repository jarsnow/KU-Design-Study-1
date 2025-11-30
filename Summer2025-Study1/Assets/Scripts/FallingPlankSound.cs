using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    float minimum_velocity_sound_threshold = 0.1f;
    public AudioClip[] plank_noises;
    AudioSource water_noise;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject collided = collision.gameObject;
        Debug.Log("new collision");
        Debug.Log("Velocity: " + GetComponent<Rigidbody>().linearVelocity);
        float velocity_magnitude = GetComponent<Rigidbody>().linearVelocity.magnitude;
        if (collided.tag == "water")
        {
            // add water noises later
        } else {
            // make wood smacking noise
            if (velocity_magnitude > minimum_velocity_sound_threshold)
            {
                AudioClip plank_noise = plank_noises[UnityEngine.Random.Range(0, plank_noises.Length - 1)];
                AudioSource source = GetComponent<AudioSource>();
                source.resource = plank_noise;

                float pitch_modifier = Random.Range(0.5f, 0.7f);
                source.pitch = pitch_modifier;

                // modify audio volume based on velocity
                Debug.Log("velocity: " + collision.relativeVelocity.magnitude.ToString());

                source.volume = Mathf.Max(0.1f, Mathf.Min(1.5f, collision.relativeVelocity.magnitude / 8));
                source.spatialBlend = 1.0f;
                source.dopplerLevel = 0;
                source.Play();
            }
        }
    }
}
