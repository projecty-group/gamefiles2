using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FCP_Lure : MonoBehaviour
{
    public float originalGravityScale = 1.0f;           // Save our original gravity

    public float waterGravityMax = 1.0f;                // The max gravity change for the lure in water
    public float waterGravityMin = -1.0f;               // The min for our change of the lure

    public GameObject splashEffect;                     // Our splash FX when the lure hits the water

    public bool isInWater = false;                      // Is our Lure in the water

    // Start is called before the first frame update
    void Start()
    {
        // Save our original gravity scale
        originalGravityScale = GetComponent<Rigidbody2D>().gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Water"))
        {
            // If we hit water play our SFX
            GetComponent<AudioSource>().Play();

            isInWater = true;

            // Spawn the splash
            GameObject go = Instantiate(splashEffect, transform.position, Quaternion.identity);
            Destroy(go, 2.0f);

            // Slow down our lure
            FCP_Manager.instance.fishingLure.gravityScale = originalGravityScale * 0.50f;
            FCP_Manager.instance.fishingLure.velocity = Vector2.zero;


            // Cut the velocity in half to "impact" water
            FCP_Manager.instance.fishingLure.velocity = GetComponent<Rigidbody2D>().velocity * 0.20f;

        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        // If we hit water
        if (collision.gameObject.CompareTag("Water"))
        {
            // Slow down our velocity
            FCP_Manager.instance.fishingLure.velocity = new Vector2(0.0f, Mathf.Clamp(FCP_Manager.instance.fishingLure.velocity.y, -1.0f, 1.0f));

        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        // If we exit water
        if (collision.gameObject.CompareTag("Water"))
        {
            isInWater = false;

            // Restore our gravity
            FCP_Manager.instance.fishingLure.gravityScale = originalGravityScale;
        }
    }
}