using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private GameObject[] laserReceivers;
    private GameObject lastHittedRecevier;
    private bool hasLaserBlockedBefore = false;

    [Header("Events")]
    public GameEvent onLaserCollided;
    public GameEvent onLaserBlocked;
    public GameEvent onLaserCollidedWithLaserCube;

    //enum Directions { north = 0, east = 90, south = 180, west  = 270 }
    //[Header("Directions")]
    //[SerializeField] private Directions directions; 

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, startPoint.position);
        direction = -transform.forward;
        //gateAnimator = gate.GetComponent<Animator>();
        GetSimilarReceviers();
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

        //if (laserReceivers.Length == 0)
        //{
        //    Debug.Log("No lasers found;");
        //    return;
        //}

        

        if (Physics.Raycast(transform.position, direction, out hit))
        {
            
            if (hit.collider)
            {
                lineRenderer.SetPosition(1, hit.point);
            }
            if (hit.transform.tag.EndsWith("LaserReceiver"))
            {

                if (laserReceivers.Contains(hit.transform.gameObject))
                {
                    lastHittedRecevier = hit.transform.gameObject;
                    Debug.Log("Open Gate Logic!");
                    onLaserCollided.Raise(this, null, lastHittedRecevier.gameObject.GetComponent<LaserReceiver>().GateNumber);
                    hasLaserBlockedBefore = false;
                    //Debug.Log("Event raiser with this color: " + hit.transform.gameObject.tag);
                   
                } else
                {
                    lastHittedRecevier = null;
                    Debug.Log("Wrong receiver color" + lastHittedRecevier);
                }

            }
            else
            {
                //Debug.Log("Close Gate Logic!");
                //Debug.Log("laser emitter gate number attached: " + laserReceivers[0].gameObject.GetComponent<LaserReceiver>().GateNumber);
                
                if (!hasLaserBlockedBefore && lastHittedRecevier != null)
                {
                    
                    onLaserBlocked.Raise(this, null, 1);
                    lastHittedRecevier = null;
                    hasLaserBlockedBefore = true;
                }
                
            }

            if (hit.transform.tag == "LaserCube")
            {
                onLaserCollidedWithLaserCube.Raise(this, null, -1);
            }

            // if hitted robot
            if (hit.transform.tag.StartsWith("Robot"))
            {
                // check if active
                if (!hit.transform.gameObject.GetComponent<PlayerController>().enabled)
                {
                    hit.transform.gameObject.GetComponent<PlayerController>().enabled = true;
                    hit.transform.gameObject.GetComponent<PlayerController>().isActive = true;
                    hit.transform.gameObject.GetComponent<PlayerController>().TurnOnRobot();
                    Debug.Log("Active robot please");
                } else // if not active
                {
                    //Debug.Log("Laser Pointing logic!");
                }
                
            }
        }
        else
        {
            lineRenderer.SetPosition(1, direction * laserDistance);
        }

    }

    private void GetSimilarReceviers()
    {
        if (this.tag.StartsWith("Blue"))
        {
            laserReceivers = GameObject.FindGameObjectsWithTag("BlueLaserReceiver");

        } else
        {
            laserReceivers = GameObject.FindGameObjectsWithTag("RedLaserReceiver");
        }
        //foreach (var lr in laserReceivers)
        //{
        //    Debug.Log(lr.tag + " " + laserReceivers.Length);
        //}
    }

}
