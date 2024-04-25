using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEmitter : MonoBehaviour
{

    private LineRenderer lineRenderer;

    [SerializeField]
    private Transform startPoint;

    [Header("Laser Distance")]
    [SerializeField]
    private int laserDistance;
    private Vector3 direction;

    [Header("Gates Attached")]
    [SerializeField]
    private GameObject gate;

    private Animator gateAnimator;

    //enum Directions { north = 0, east = 90, south = 180, west  = 270 }
    //[Header("Directions")]
    //[SerializeField] private Directions directions; 

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, startPoint.position);
        direction = -transform.forward;
        gateAnimator = gate.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(startPoint.position);
        //lineRenderer.SetPosition(0, new Vector3(0, 0, 0));
        RaycastHit hit;
        // this will be for north by default

        //if (directions == Directions.south)
        //{
        //    direction = transform.forward;
        //} else if (directions == Directions.east)
        //{
        //    direction = transform.right;
        //} else if  (directions == Directions.west)
        //{
        //    direction = -transform.right;
        //}

        if (Physics.Raycast(transform.position, direction, out hit))
        {

            if (hit.collider)
            {
                lineRenderer.SetPosition(1, hit.point);

            }

            if (hit.transform.tag == "LaserReceiver")
            {
                gateAnimator.Play("Doors1|Open1", 1);
                gateAnimator.Play("Doors2|Open2", 2);
                //gateAnimator.ResetTrigger("open");
                gateAnimator.SetBool("isOpened", true);
                //Debug.Log("Open Gate Logic!");
            }
            else
            {
                gateAnimator.Play("Doors1|Close1", 1);
                gateAnimator.Play("Doors2|Close2", 2);
                gateAnimator.SetBool("isOpened", false);
                //gateAnimator.SetTrigger("open");
                //Debug.Log("Close Gate Logic!");
            }


            if (hit.transform.tag.StartsWith("Robot"))
            {
                Debug.Log("Laser Pointing logic!");
            }
        }
        else
        {
            lineRenderer.SetPosition(1, direction * laserDistance);
        }
    }
}
