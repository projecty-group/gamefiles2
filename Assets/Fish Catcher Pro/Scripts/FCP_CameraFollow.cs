using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FCP_CameraFollow : MonoBehaviour
{
    public Transform mainTarget;        // Our player
    public Transform secondaryTarget;   // Our lure / secondary target

    public Vector3 cameraOffset;        // The offset we need to maintain
    public float followSpeed = 5.0f;    // How fast we follow the target

    // Start is called before the first frame update
    void Start()
    {
        // Find our main target which is the player
        mainTarget = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        // If our secondary target is not assign, use our player
        if (secondaryTarget == null)
        {
            transform.position = mainTarget.position + cameraOffset;
        }
        else
        {
            transform.position = secondaryTarget.position + cameraOffset;

        }
    }
}
