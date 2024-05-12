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
            ILaserInteractable currentObject = currentHitObject.GetComponent<ILaserInteractable>();
            ILaserInteractable lastObject = lastHitObject.GetComponent<ILaserInteractable>();
            //Debug.Log($"Currenthit object: {currentHitObject}, Lasthit object: {lastHitObject}, Last object: {lastObject}");
            if (currentObject != null)
            {
                currentObject.LaserCollide(this);
                hitLaserInteractable = true;
            } 
            if (lastObject != null && lastHitObject != currentHitObject)
            {
                //Debug.Log($"Exiting last object: {lastHitObject}");
                lastObject.LaserExit(this);
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
            if (lastHitObject != null && lastHitObject.GetComponent<ILaserInteractable>() != null)
            {
                lastHitObject.GetComponent<ILaserInteractable>().LaserExit(this);
            }
        }
    }
    

    // this will block laser from any object hit in the last frame
    public void BlockLaser()
    {
        if (!lastHitObject) return;
        ILaserInteractable lastObject = lastHitObject.GetComponent<ILaserInteractable>();
        if (lastObject == null ) return;
        currentHitObject = dummyGameObject;
        lastObject.LaserExit(this);
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
    }
}
