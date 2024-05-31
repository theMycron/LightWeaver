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
        lineRenderer.enabled = true;
        //HideCollisionEffect();
    }

    private void OnDisable()
    {
        lineRenderer.enabled = false;
        BlockLaser();
        HideCollisionEffect();
    }

    void Start()
    {
        dummyGameObject = gameObject;
        lineRenderer = GetComponent<LineRenderer>();
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
        FireRaycast();
    }
    
    private void FireRaycast()
    {
        RaycastHit[] hits = Physics.RaycastAll(startPoint.position, direction, 100f);
        lineRenderer.SetPosition(0, startPoint.position);

        if (hits != null && hits.Length > 0)
        {
            RaycastHit closestHit = GetClosestHit(hits);
            if (closestHit.transform.gameObject != this.gameObject)
            {
                HandleLaserCollision(closestHit);
                return;
            }
            // if didnt hit anything, extend laser renderer and exit from the last hit object
            lineRenderer.SetPosition(1, startPoint.position + direction * laserDistance);
            if (lastHitObject != null && lastHitObject.GetComponent<ILaserInteractable>() != null)
            {
                lastHitObject.GetComponent<ILaserInteractable>().LaserExit(this);
            }
        }
    }

    // get all objects the raycast hit, and make sure to ignore
    // a hit if it hit the source of this laser
    private RaycastHit GetClosestHit(RaycastHit[] hits)
    {
        RaycastHit hit = hits[0];
        float minDistance = 100f;
        foreach (RaycastHit h in hits)
        {
            // ignore self
            if (h.transform.gameObject != this.gameObject)
            {
                // get closest hit
                float distance = Mathf.Abs((h.transform.position - transform.position).magnitude);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    hit = h;
                }
            }
        }
        return hit;
    }

    private void HandleLaserCollision(RaycastHit hit)
    {
        if (hit.transform.gameObject == this.gameObject) return;

        bool hitLaserInteractable = false;

        if (hit.collider)
        {
            //Debug.Log("I have hit by: " +  hit.collider);

            lineRenderer.SetPosition(1, hit.point);

            currentHitObject = hit.collider.gameObject;
            if (currentHitObject == this.gameObject) { Debug.Log("Laser hit its own source object!"); }
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
        }
        else
        {
            SetCollisionEffect(hit.point, hit.normal);
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
