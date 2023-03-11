using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FCP_Fish : MonoBehaviour
{
    [Header("Fish POI & Positions")]
    public GameObject[] fishPOI;                        // Our points of interest that our fish have
    public GameObject[] fishExitPOI;                    // Our exit points that fish swim towards when escaping
    private Transform currentPOI;                       // Our current point of interest
    private Vector3 oldPosition;                        // Our previous position

    [Header("Fish Settings")]
    public bool recentlyCaught = false;                 // Track if we have been recently caught
    public bool isCaught = false;                       // Are we caught?
    public bool chaseLure = false;                      // Are we chasing a lure

    public float swimSpeed = 1.0f;  
    public float fishLife = 0.50f;

    public float breakAwayPower = 0.05f;                // The power that our fish can fight against the player
    public float breakAwayTimer = 1.0f;                 // This is the value used to see how fast the fish applies the break away power

    [HideInInspector]
    public GameObject lure;                             // The lure in the water

    public float lureAttractionDistance = 2.0f;         // How far our fish can "see" the lure  

    [Range(1, 10)]
    public float attractionChance = 1;                  // The chance rate of our fish wanting the lure

    public bool isAttacking = false;                    // Are we attacking the lure?

    [Header("Value Settings")]
    public int coinValue = 1;                           // The fishes value

    private void Awake()
    {
        // Find our POIs
        fishPOI = GameObject.FindGameObjectsWithTag("Fish_POI");
        fishExitPOI = GameObject.FindGameObjectsWithTag("Fish_Exit_POI");
    }

    private void Start()
    {
        // Find the lure
        lure = GameObject.FindGameObjectWithTag("Lure");

        // Load our POIs
        FindPOI();
        
        // Setup our initial values
        FCP_Manager.instance.uiFishBattlePopupSlider.value = fishLife;
    }

    private void Update()
    {
        // As long as we're NOT caught
        if (!isCaught)
        {
            // Rotate the fish to the catch rotation
            transform.localRotation = Quaternion.Euler(0, 0, 0);

            // Check for direction of sprite
            if (oldPosition.x < transform.localPosition.x)
            {
                GetComponent<SpriteRenderer>().flipX = false;

            }
            else if (oldPosition.x > transform.localPosition.x)
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }

            oldPosition = transform.localPosition;

            // Are we within biting distance of the lure
            if (Vector2.Distance(transform.position, lure.transform.position) <= lureAttractionDistance && !recentlyCaught && lure.GetComponent<FCP_Lure>().isInWater)
            {
                // Do we want to bite it?
                if (Random.Range(1, 10) <= attractionChance)
                {
                    // Yes we do want to bite it, swim towards the lure now
                    SwimToPOI(lure.transform);

                    // Are we close enought to actually bite the lure now?
                    if (Vector2.Distance(transform.position, lure.transform.position) <= 0.50f && !isCaught)
                    {
                        isCaught = true;

                        FCP_Manager.instance.CaughtFish(this);
                    }
                }
            }
            else
            {
                // Just keep swimming
                if (!DidWeArriveAtPOI(currentPOI))
                {
                    SwimToPOI(currentPOI);
                }
                else
                {
                    FindPOI();
                }
            }
        }
        else
        {
            // We've been caught!

            // Update the fish slider
            FCP_Manager.instance.uiFishBattlePopupSlider.value = fishLife;
            
            // Disable flipping if we're caught
            GetComponent<SpriteRenderer>().flipX = false;

            // Rotate the fish to the catch rotation
            transform.localRotation = Quaternion.Euler(0, 0, 15);
        }
    }

    /// <summary>
    /// FindPOI() - Find a new fish POI
    /// </summary>
    public void FindPOI()
    {
        if (!isAttacking)
        {
            currentPOI = fishPOI[Random.Range(0, fishPOI.Length)].transform;
        }
    }

    /// <summary>
    /// SwimToPOI() - Move towards a POI
    /// </summary>
    /// <param name="poi"></param>
    private void SwimToPOI(Transform poi)
    {
        float step = swimSpeed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, poi.position, step);

    }

    /// <summary>
    /// DidWeArriveAtPOI() - Verify if we're at a POI
    /// </summary>
    /// <param name="poi"></param>
    /// <returns></returns>
    private bool DidWeArriveAtPOI(Transform poi)
    {
        if (Vector2.Distance(transform.position, poi.position) <= 0.50f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// SetNewPOI() - Change our current POI
    /// </summary>
    /// <param name="newPOI"></param>
    public void SetNewPOI(Transform newPOI)
    {
        currentPOI = newPOI;
    }

    /// <summary>
    /// Draw our wire sphere to show our lure attraction distance
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, lureAttractionDistance);
    }

    /// <summary>
    /// RecentlyCaughtCooldown() - This allows us to enable a cooldown the fish cannot be caught back to back.
    /// </summary>
    public void RecentlyCaughtCooldown()
    {
        recentlyCaught = true;

        StartCoroutine(RecentlyCaughtTimer());
    }

    // Setup a basic timer
    IEnumerator RecentlyCaughtTimer()
    {
        yield return new WaitForSeconds(5.0f);

        recentlyCaught = false;

        fishLife = 0.50f;
    }
}
