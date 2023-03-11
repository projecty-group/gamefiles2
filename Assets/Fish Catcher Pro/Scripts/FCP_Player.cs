using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FCP_Player : MonoBehaviour
{
    public bool startingCast = false;                   // Set when we begin to drag / cast
    public bool releasedLine = false;                   // Set when we release, used to manage dragging
    public bool lureAtHome = true;                      // Is the lure reeled all the way in

    public float autoReelInDistance = 2.0f;             // This distance is when we get the lure close to the home position, just auto reel it back in

    public FCP_CameraFollow cameraFollow;               // Our camera manager
    public Transform boat;                              // That's my boat
   
    public float throwForce = 0.3f;                     // The force at which we cast the lure


    // These variables are used for touch position tracking and casting
    private Vector2 startPos;
    private Vector2 endPos;
    private Vector2 direction;
    private float touchTimeStart;
    private float touchTimeFinish;
    private float timeInterval;

    [Header("Cast Animation")]
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        //playerArmRotZ = playerArmLeft.rotation.z;   
        Input.simulateMouseWithTouches = true;
        cameraFollow = Camera.main.GetComponent<FCP_CameraFollow>();
    }

    // Update is called once per frame
    void Update()
    {
        // If we're NOT in a fishing battle
        if (!FCP_Manager.instance.isCatchingFishInBattle)
        {
            // Is the lure at the home position
            if (lureAtHome)
            {
                // If we have the left mouse down or single touch
                if (Input.GetMouseButton(0))
                {       
                    // And we're not starting to cast
                    if(!startingCast)
                    {
                        // Begin our time marker and tracking for casting
                        touchTimeStart = Time.time;
                        startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                        startingCast = true;
                    }
                }

                // If we release the left mouse button
                if (Input.GetMouseButtonUp(0))
                {
                    startingCast = false;

                    // Mark our drag distance and timer
                    touchTimeFinish = Time.time;
                    timeInterval = touchTimeFinish - touchTimeStart;
                    endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    direction = startPos - endPos;

                    // If we have a cast based on the drag length of the finger
                    if (direction.magnitude > 3.0f)
                    {
                        // Cast!
                        animator.SetTrigger("Cast");
                        
                        // Play our SFX for cast
                        GetComponent<AudioSource>().Play();

                        // Reset our variables
                        releasedLine = true;
                        lureAtHome = false;

                        // Detach the lure from our rod so it can be a physics object and react with the water
                        FCP_Manager.instance.fishingLure.transform.parent = null;
                        FCP_Manager.instance.fishingLure.simulated = true;

                        // Follow the lure now and NOT our player
                        cameraFollow.secondaryTarget = FCP_Manager.instance.fishingLure.transform;

                        // Mod the gravity scale of the lure, this makes it feel better when casting
                        FCP_Manager.instance.fishingLure.gravityScale = 1.00f;

                        // Add some force 
                        FCP_Manager.instance.fishingLure.AddForce(-direction * throwForce);
                            
                    }
                }
            }
            else
            {
                // If we're holding  LMB while we're NOT at the home position, we must be reeling in the lure now
                if (Input.GetMouseButton(0))
                {
                    // Reel in
                    float step = FCP_Manager.instance.reelSpeed * Time.deltaTime;

                    // Play reel in SFX
                    if( !FCP_Manager.instance.fcpPlayer.GetComponent<AudioSource>().isPlaying )
                        FCP_Manager.instance.fcpPlayer.GetComponent<AudioSource>().PlayOneShot(FCP_Manager.instance.reelingInSFX);

                    // Move the position of the lure towards our castFromLocation which is at the fishing rod
                    FCP_Manager.instance.fishingLure.transform.position = Vector2.MoveTowards(FCP_Manager.instance.fishingLure.transform.position, FCP_Manager.instance.castFromLocation.position, step);
                    releasedLine = true;

                    // Check if we're at home
                    if (Vector2.Distance(FCP_Manager.instance.fishingLure.transform.position, FCP_Manager.instance.castFromLocation.position) <= autoReelInDistance)
                    {
                        // Stop playing sounds if we're at home
                        FCP_Manager.instance.fcpPlayer.GetComponent<AudioSource>().Stop();

                        // Reset our lure position now
                        FCP_Manager.instance.fishingLure.transform.position = FCP_Manager.instance.castFromLocation.position;

                        // This allow us to STOP listening for inputs for one frame so we can resync back to the cast
                        Input.ResetInputAxes();

                        // Reparent
                        FCP_Manager.instance.fishingLure.transform.SetParent(FCP_Manager.instance.castFromLocation);
                        FCP_Manager.instance.fishingLure.simulated = false;

                        // Camera follows our player (boat)
                        cameraFollow.mainTarget = boat;

                        lureAtHome = true;
                        releasedLine = false;
                    }
                }
                else
                {
                    FCP_Manager.instance.fcpPlayer.GetComponent<AudioSource>().Stop();
                }
            }
        }
        else if(FCP_Manager.instance.isCatchingFishInBattle)
        {
            // If we're battling a fish, monitor LMB for clicking (same for touch)
            if(Input.GetMouseButton(0))
            {
                // If we have a failure rate, this is just a random failure to help make it more challenging. 
                // Future update, move this failure rate into the fish prefab.
                if (Random.Range(0, 10) > 5)
                {
                    Debug.Log("Failed reeling!");

                    Input.ResetInputAxes();
                }
                else
                {
                    // Move fish closer to our player
                    float step = (FCP_Manager.instance.reelAttackPower + FCP_Manager.instance.reelAttackPower) * Time.deltaTime;
                    FCP_Manager.instance.fishingLure.transform.position = Vector2.MoveTowards(FCP_Manager.instance.fishingLure.transform.position, FCP_Manager.instance.castFromLocation.transform.position, step);

                    // Make some "damage" on the fish so we can catch it
                    if(FCP_Manager.instance.currentFish)
                        FCP_Manager.instance.currentFish.fishLife += FCP_Manager.instance.reelFishAttackPower;
                }
            }
        }

    }

    /// <summary>
    /// This tracks to see if we actually pulled a fish back to the boat itself. If we did, we get coins!
    /// </summary>
    /// <param name="collision"></param>
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Fish"))
        {
            FCP_Manager.instance.ScoreFish(collision.GetComponent<FCP_Fish>());
        }
    }
}
