using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCube : MonoBehaviour, ILaserInteractable
{
    [SerializeField]
    private Material redMaterial;

    [SerializeField] 
    private Material blueMaterial;

    public Color color;
    public LaserColors colorEnum;

    private LineRenderer lineRenderer;

    [SerializeField]
    private Transform startPoint;
    private Laser laserScript;


    [SerializeField]
    private bool isActive;

    private GameObject activatedBy;


    // Start is called before the first frame update
    void Start()
    {
        isActive = false;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, startPoint.position);
        //lineRenderer.useWorldSpace = true;
        lineRenderer.enabled = false;
        laserScript = GetComponent<Laser>();

        ChangeCubeColor(colorEnum);
        ToggleLaserCube(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (laserScript.enabled)
        {
            isActive = true;
            lineRenderer.SetPosition(0,startPoint.position);
            laserScript.direction = transform.forward;
        }
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

    private void ChangeCubeColor(LaserColors color)
    {
        colorEnum = color;
        if (color == LaserColors.blue)
        {
            // change color of inner sphere
            transform.GetChild(1).GetComponent<MeshRenderer>().material = blueMaterial;
        } else if (color == LaserColors.red)
        {
            // change color of inner sphere
            transform.GetChild(1).GetComponent<MeshRenderer>().material = redMaterial;
        }
        this.color = (Color) Colors.GetLaserColor(color);
    }


    private void ToggleLaserCube(bool value)
    {
        if (value)
        {
            Debug.Log("Redirect Laser called!");
            isActive = true;
            ChangeCubeColor(activatedBy.GetComponent<Laser>().colorEnum);
            if (!laserScript.enabled)
            {
                laserScript.enabled = true;
                laserScript.SetLaserColor(colorEnum);
                laserScript.direction = transform.forward;
                //Debug.Log("Direction: "+laserScript.direction);
                //Debug.Log("Rotation: "+transform.forward);
            }
        } else
        {
            isActive = false;
            //ToggleLaserCube(isActive);
            laserScript.enabled = false;

            Debug.Log($"laser cube stopped redirecting");
        }
    }

    public void LaserCollide(Laser sender)
    {
        if (!isActive && activatedBy == null)
        {
            Debug.Log("Laser entered cube");
            activatedBy = sender.gameObject;
            ToggleLaserCube(true);
        }
    }
    public void LaserExit(Laser sender)
    {
        if (isActive && activatedBy == sender.gameObject)
        {
            Debug.Log("Laser exited cube");
            activatedBy = null;
            ToggleLaserCube(false);
        }
    }
}
