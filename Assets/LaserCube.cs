using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCube : MonoBehaviour, IActivable, IDisable
{
    [SerializeField]
    private Material redMaterial;

    [SerializeField] 
    private Material blueMaterial;

    public bool isBlue = false;

    private LineRenderer lineRenderer;

    [SerializeField]
    private Transform startPoint;
    private Laser laserScript;

    [SerializeField]
    private bool isActive;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, startPoint.position);
        //lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = 0.4059853f;
        lineRenderer.endWidth = 0.4059853f;
        lineRenderer.enabled = false;


        laserScript = GetComponent<Laser>();
    }

    // Update is called once per frame
    void Update()
    {
        if (laserScript.enabled)
        {
            lineRenderer.SetPosition(0,startPoint.position);
            laserScript.direction = transform.forward;
        }
    }

    public void RedirectLaser(Component sender, object data, int t)
    {
        Debug.Log("Redirect Laser called!");
        
        if (!lineRenderer.enabled)
        {
            lineRenderer.enabled = true;
            laserScript.enabled = true;
            laserScript.isBlue = isBlue;
            laserScript.direction = transform.forward;
            Debug.Log("Direction: "+laserScript.direction);
            Debug.Log("Rotation: "+transform.forward);
        }
        ChangeInnerSphereColor(sender);

        //Debug.Log("This is the starting point: " + startPoint.position);
        //Debug.Log("This is the ending point: " + lineRenderer.GetPosition(1));
        //Debug.DrawLine(startPoint.localPosition, lineRenderer.GetPosition(1), Color.blue);
        //Debug.Log("This is the direction: " + (lineRenderer.GetPosition(1) - startPoint.localPosition));

        //RaycastHit hit;
        //if (Physics.Linecast(startPoint.position, lineRenderer.GetPosition(1), out hit))
        //{

        //    if (hit.collider)
        //    {
        //        Debug.Log("I have hitted something: " + hit.collider.tag);
        //        lineRenderer.SetPosition(1, hit.point);
        //    }
        //}
    }
    public void StopLaser(Component sender, object data, int t)
    {
        Debug.Log("Stop Laser called!");
        if (lineRenderer.enabled)
        {
            lineRenderer.enabled = false;
            laserScript.enabled = false;
        }

        //Debug.Log("is enabled: ");
        //Debug.Log(lineRenderer.enabled);
        //Debug.Log(laserScript.enabled);
    }

    private void ChangeInnerSphereColor(Component sender)
    {
        
        if (sender.tag.StartsWith("Blue"))
        {
            GameObject.Find("InnerSphere").gameObject.GetComponent<MeshRenderer>().material = blueMaterial;
            lineRenderer.material = blueMaterial;
            isBlue = true;
        } else
        {
            GameObject.Find("InnerSphere").gameObject.GetComponent<MeshRenderer>().material = redMaterial;
            lineRenderer.material = redMaterial;
            isBlue = false;
        }
        
    }

    public void Activate()
    {
        isActive = true;
        if (!lineRenderer.enabled)
        {
            ToggleLaserCube(isActive);
            laserScript.isBlue = isBlue;
            laserScript.direction = transform.forward;
            Debug.Log("Direction: "+laserScript.direction);
            Debug.Log("Rotation: "+transform.forward);
        }
        //ChangeInnerSphereColor(sender);
        

    }

    public void Deactivate()
    {
        isActive = false;
        ToggleLaserCube(isActive);
    }

    private void ToggleLaserCube(bool isActive)
    {
        laserScript.enabled = isActive;
        lineRenderer.enabled = isActive;
    }
}
