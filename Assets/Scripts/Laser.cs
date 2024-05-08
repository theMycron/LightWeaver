using System.Collections;
using System.Collections.Generic;
//using System.Drawing;
using System.Linq;
using UnityEngine;

public class Laser : MonoBehaviour
{

    private LineRenderer lineRenderer;
    [SerializeField] public GameObject collisionEffect;
    public Transform startPoint;
    public Vector3 direction;
    public LaserColors colorEnum;
    public Color color;
    private int laserDistance = 1000;

    [Header("Events")]
    public GameEvent onLaserCollided;
    public GameEvent onLaserBlocked;
    public GameEvent onLaserStopped;
    public GameEvent onLaserCollidedWithLaserCube;

    private GameObject[] laserReceivers;
    //private GameObject lastHittedRecevier;
    //private GameObject lastHittedLaserCube;
    //private bool hasLaserBlockedBefore = false;

    private GameObject currentHitObject;
    private GameObject lastHitObject;
    private GameObject dummyGameObject;
    private GameObject collisionEffectInstance;

    private void OnEnable()
    {
        lineRenderer = GetComponent<LineRenderer>();
        //HideCollisionEffect();
    }

    void Start()
    {
        dummyGameObject = gameObject;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, startPoint.position);
        collisionEffectInstance = Instantiate(collisionEffect);
        collisionEffect.SetActive(false);

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
            bool hitLaserInteractable = false;

            if (hit.collider)
            {
                //Debug.Log("I have hit by: " +  hit.collider);

                lineRenderer.SetPosition(1, hit.point);

                currentHitObject = hit.collider.gameObject;
                if (!lastHitObject) { lastHitObject = currentHitObject; }
            }

            if (currentHitObject.tag.EndsWith("LaserReceiver"))
            {

                if (laserReceivers.Contains(currentHitObject))
                {
                    int receiverNumber = currentHitObject.GetComponent<LaserReceiver>().ReceiverNumber;
                    Debug.Log("Receiver hit by correct laser! Raise onLaserCollided Event to receiver #"+ receiverNumber);
                    //onLaserCollided.Raise(this, null, lastHittedRecevier.gameObject.GetComponent<LaserReceiver>().GateNumber);

                    onLaserCollided.Raise(this, receiverNumber, "Receiver", null);

                    //hasLaserBlockedBefore = false;
                    //Debug.Log("Event raiser with this color: " + hit.transform.gameObject.tag);
                    hitLaserInteractable = true;
                }
                else
                {
                    Debug.Log("Wrong receiver color: " + lastHitObject);
                }

            }
            else
            {
                //Debug.Log("Close Gate Logic!");
                //Debug.Log("laser emitter gate number attached: " + laserReceivers[0].gameObject.GetComponent<LaserReceiver>().GateNumber);

                BlockLaserFromReceiver();
            }

            if (currentHitObject.tag == "LaserCube")
            {
                int laserCubeNumber = currentHitObject.GetComponent<LaserCube>().CubeNumber;
                //onLaserCollidedWithLaserCube.Raise(this, null, -1);
                //lastHittedLaserCube = hit.transform.gameObject;
                //Debug.Log("Hit laserCube! Raise onLaserCollided Event to cube #" + laserCubeNumber);
                onLaserCollidedWithLaserCube.Raise(this, laserCubeNumber, "LaserCube", null);
                hitLaserInteractable = true;
            } else
            {
                //if (lastHittedLaserCube != null)
                BlockLaserFromLaserCube();
            }

            // if hitted robot
            if (currentHitObject.tag.StartsWith("Robot"))
            {
                Debug.Log("HIT ROBOT WITH LASER " + startPoint.position);
                // TODO: change behaviour to perform laser pointing logic only when robot is active
                // robot activation should have the same behaviour as opening a gate,
                // either by floor button or laser receiver
                // check if active
                if (!currentHitObject.GetComponent<PlayerController>().isActive)
                {
                    hitLaserInteractable = true;
                    currentHitObject.GetComponent<PlayerController>().ActivateRobot();
                    //Debug.Log("Active robot please");
                }
                else // if not active
                {
                    //Debug.Log("Laser Pointing logic!");
                }

            }

            if (currentHitObject.tag.Equals("Dummy"))
            {
                if (colorEnum == LaserColors.red)
                {
                    Debug.Log("Kill and explode dummy");
                } else
                {
                    Debug.Log("Freeze dummy");
                }
            } else
            {
                BlockLaserFromDummy();
            }

            lastHitObject = currentHitObject;

            if (hitLaserInteractable)
            {
                HideCollisionEffect();
            } else
            {
                SetCollisionEffect(hit.point, hit.normal);
            }
        }
        else
        {
            lineRenderer.SetPosition(1, startPoint.position + direction * laserDistance );
        }
    }

    public void BlockLaserFromLaserCube()
    {
        if (!lastHitObject) { return; }
        LaserCube lastHitLaserCube = lastHitObject.GetComponent<LaserCube>();
        LaserCube currentHitLaserCube = currentHitObject.GetComponent<LaserCube>();
        bool didntHitLaserCubeBefore = !lastHitLaserCube;
        //Debug.Log("trying to block lasercube, did we hit laser cube now? "+(currentHitLaserCube != null));
        // if a laser cube was hit last frame AND this frame BUT they are different numbers then raise blocked event
        if (didntHitLaserCubeBefore || (currentHitLaserCube && lastHitLaserCube.CubeNumber == currentHitLaserCube.CubeNumber))
        {
            return;
        }
        //Debug.Log("onLaserStopped is raised for laser cube number " + lastHitLaserCube.CubeNumber);
        //onLaserBlocked.Raise(this, null,-1);
        onLaserStopped.Raise(this, lastHitLaserCube.CubeNumber, "LaserCube", null);
        
    }

    // used to raise laser blocked event for receiver if it was hit in the last frame
    // but not in this frame
    public void BlockLaserFromReceiver()
    {
        if (!lastHitObject) { return; }
        LaserReceiver lastHitReceiver = lastHitObject.GetComponent<LaserReceiver>();
        LaserReceiver currentHitReceiver = currentHitObject.GetComponent<LaserReceiver>();
        bool didntHitReceiverBefore = !lastHitReceiver;
        //Debug.Log($"trying to block laser from receiver {lastHitObject}");

        // dont continue if last hit receiver doesnt exist
        // or if it was different color
        // or if the last hit receiver is the same as the current hit one
        if (didntHitReceiverBefore || lastHitReceiver.selectedLaserColor != colorEnum
            || (currentHitReceiver && lastHitReceiver.ReceiverNumber == currentHitReceiver.ReceiverNumber))
        {
            return;
        }

        onLaserBlocked.Raise(this, lastHitReceiver.ReceiverNumber, "Receiver", null);
        //onLaserBlocked.Raise(this, null, 1);
        //hasLaserBlockedBefore = true;
        
    }

    public void BlockLaserFromDummy()
    {
        if (!lastHitObject) { return; }
        BasicDummy lastDummy = lastHitObject.GetComponent<BasicDummy>();
        BasicDummy currentDummy = currentHitObject.GetComponent<BasicDummy>();

        if (!lastDummy || (currentDummy && lastDummy.DummyNumber == currentDummy.DummyNumber))
        {
            return;
        }


        onLaserBlocked.Raise(this, lastDummy.DummyNumber, "Dummy", null);

    }

    // this will block laser from any object hit in the last frame
    public void BlockLaserFromAll()
    {
        currentHitObject = dummyGameObject;
        BlockLaserFromLaserCube();
        BlockLaserFromReceiver();
    }
    private void SetCollisionEffect(Vector3 position, Vector3 normal)
    {
        LaserCollision laserCollision = collisionEffectInstance.GetComponent<LaserCollision>();
        laserCollision.color = color;
        collisionEffectInstance.SetActive(true);
        // add offset to prevent clipping
        laserCollision.MoveCollision(position + (normal*0.2f), Quaternion.LookRotation(normal));
        Debug.DrawRay(collisionEffectInstance.transform.position, collisionEffect.transform.forward*5, Color.blue, 1, false);
    }
    public void HideCollisionEffect()
    {
        if (!collisionEffectInstance)
        {
            return;
        }
        collisionEffectInstance.SetActive(false);
    }

    public void SetLaserColor(LaserColors color)
    {
        colorEnum = color;
        this.color = (Color)Colors.GetLaserColor(color);
        // Line renderer needs a gradient, so create one with only the passed in color
        Gradient gradient = new Gradient();

        GradientColorKey[] colors = new GradientColorKey[2];
        colors[0] = new GradientColorKey(this.color, 1.0f);
        colors[1] = new GradientColorKey(this.color, 1.0f);

        GradientAlphaKey[] alphas = new GradientAlphaKey[2];
        alphas[0] = new GradientAlphaKey(1.0f, 1.0f);
        alphas[1] = new GradientAlphaKey(1.0f, 1.0f);

        gradient.SetKeys(colors, alphas);

        Debug.Log($"laserscript new color: {this.color}");

        lineRenderer.colorGradient = gradient;
        GetSimilarReceviers();
    }

    private void GetSimilarReceviers()
    {
        if (colorEnum == LaserColors.blue)
        {
            laserReceivers = GameObject.FindGameObjectsWithTag("BlueLaserReceiver");
        } else if (colorEnum == LaserColors.red)
        {
            laserReceivers = GameObject.FindGameObjectsWithTag("RedLaserReceiver");
        }
    }
}
