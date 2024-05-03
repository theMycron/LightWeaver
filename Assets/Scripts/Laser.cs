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
    public GameEvent onLaserCollidedWithLaserCube;

    private GameObject[] laserReceivers;
    private GameObject lastHittedRecevier;
    private bool hasLaserBlockedBefore = false;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, startPoint.position);
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
                    onLaserCollided.Raise(this, null, lastHittedRecevier.gameObject.GetComponent<LaserReceiver>().GateNumber);
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
            } else
            {
                Debug.Log("onLaserBlocked should be raiserd");
                onLaserBlocked.Raise(this, null,-1);
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
