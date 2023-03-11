using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FCP_FIshingLine : MonoBehaviour
{
    public Transform castFromSpot;              // This is the location of the end of the fishing rod (where we cast from)
    public LineRenderer lineRenderer;           // Our line renderer which shows us the line
    public GameObject fishingLure;              // The lure itself
    public int lineLength = 20;                 // The length of the line we're drawing

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        Vector3 ropeStartPoint = castFromSpot.position;         // Save our ropeStart position as the cast from location 
        lineRenderer.positionCount = lineLength;                // This defines how many positions we have
    }

    // Update is called once per frame
    void Update()
    {
        DrawLine();
    }

    private void DrawLine()
    {
        // Draw a basic line from our starting point to the lure itself
        lineRenderer.SetPosition(0, castFromSpot.transform.position);
        lineRenderer.SetPosition(lineLength - 1, fishingLure.transform.position);
    }
}
