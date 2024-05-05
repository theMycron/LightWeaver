using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Laser : MonoBehaviour
{

    private LineRenderer lineRenderer;
    public Transform startPoint;
    public Vector3 direction;
    public bool isBlue;
    private int laserDistance = 1000;

    [Header("Events")]
    public GameEvent onLaserCollided;
    public GameEvent onLaserBlocked;
    public GameEvent onLaserStopped;
    public GameEvent onLaserCollidedWithLaserCube;

    private GameObject[] laserReceivers;
    private GameObject lastHittedRecevier;
    private GameObject lastHittedLaserCube;
    private bool hasLaserBlockedBefore = false;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, startPoint.position);

        //lineRenderer.startWidth = 0.5f;
        //lineRenderer.endWidth = 0.5f;
        GetSimilarReceviers();
    }


    // Update is called once per frame
    void Update()
    {

        //Debug.Log("This is the starting point: " + startPoint.position);
        //Debug.Log("This is the ending point: " + lineRenderer.GetPosition(1));
        //Debug.Log("This is the direction: " + (lineRenderer.GetPosition(1) - startPoint.localPosition));
        //Debug.DrawRay(startPoint.position,direction*10, Color.blue, 3);
        //Debug.Log(startPoint.position);
        //lineRenderer.SetPosition(0, new Vector3(0, 0, 0));
        RaycastHit hit;

        if (Physics.Raycast(startPoint.position, direction, out hit))
        {

            if (hit.collider)
            {
                Debug.Log("I have hit by: " +  hit.collider);
                lineRenderer.SetPosition(1, hit.point);
            }
            if (hit.transform.tag.EndsWith("LaserReceiver"))
            {

                if (laserReceivers.Contains(hit.transform.gameObject))
                {
                    lastHittedRecevier = hit.transform.gameObject;
                    Debug.Log("Open Gate Logic!");
                    //onLaserCollided.Raise(this, null, lastHittedRecevier.gameObject.GetComponent<LaserReceiver>().GateNumber);
                    onLaserCollided.Raise(this, lastHittedRecevier.gameObject.GetComponent<LaserReceiver>().GateNumber, "Gate", null);
                    onLaserCollided.Raise(this, lastHittedRecevier.gameObject.GetComponent<LaserReceiver>().RobotNumber, "Robot", null);
                    onLaserCollided.Raise(this, lastHittedRecevier.gameObject.GetComponent<LaserReceiver>().EmitterNumber, "Emitter", null);

                    hasLaserBlockedBefore = false;
                    //Debug.Log("Event raiser with this color: " + hit.transform.gameObject.tag);

                }
                else
                {
                    lastHittedRecevier = null;
                    Debug.Log("Wrong receiver color" + lastHittedRecevier);
                }

            }
            else
            {
                //Debug.Log("Close Gate Logic!");
                //Debug.Log("laser emitter gate number attached: " + laserReceivers[0].gameObject.GetComponent<LaserReceiver>().GateNumber);

                BlockLaserFromReceiver();
                

            }

            if (hit.transform.tag == "LaserCube")
            {
                //onLaserCollidedWithLaserCube.Raise(this, null, -1);
                lastHittedLaserCube = hit.transform.gameObject;
                onLaserCollidedWithLaserCube.Raise(this, lastHittedLaserCube.gameObject.GetComponent<LaserCube>().CubeNumber, "LaserCube", null);
                
            } else
            {
                if (lastHittedLaserCube != null)
                {
                    Debug.Log("onLaserStopped should be raised");
                    //onLaserBlocked.Raise(this, null,-1);
                    onLaserStopped.Raise(this, lastHittedLaserCube.gameObject.GetComponent<LaserCube>().CubeNumber, "LaserCube", null);
                    lastHittedLaserCube = null;
                }

        }

            // if hitted robot
            if (hit.transform.tag.StartsWith("Robot"))
            {
                // TODO: change behaviour to perform laser pointing logic only when robot is active
                // robot activation should have the same behaviour as opening a gate,
                // either by floor button or laser receiver
                // check if active
                if (!hit.transform.gameObject.GetComponent<PlayerController>().isActive)
                {
                    hit.transform.gameObject.GetComponent<PlayerController>().ActivateRobot();
                    Debug.Log("Active robot please");
                }
                else // if not active
                {
                    //Debug.Log("Laser Pointing logic!");
                }

            }
        }
        else
        {
            lineRenderer.SetPosition(1, startPoint.position + direction * laserDistance );
        }
    }

    public void BlockLaserFromReceiver()
    {
        if (!hasLaserBlockedBefore && lastHittedRecevier != null)
        {
            Debug.Log("onlaserblocked is reaised");
            //onLaserBlocked.Raise(this, null, 1);
            onLaserBlocked.Raise(this, lastHittedRecevier.gameObject.GetComponent<LaserReceiver>().GateNumber, "Gate", null);
            lastHittedRecevier = null;
            hasLaserBlockedBefore = true;
        }
    }

    private void GetSimilarReceviers()
    {
        if (isBlue)
        {
            laserReceivers = GameObject.FindGameObjectsWithTag("BlueLaserReceiver");
        } else
        {
            laserReceivers = GameObject.FindGameObjectsWithTag("RedLaserReceiver");
        }
    }
}
