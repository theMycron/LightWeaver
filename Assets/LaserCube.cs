using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCube : MonoBehaviour, IActivable, IDisable
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

    [Header("LaserCube Number")]
    [SerializeField]
    private int cubeNumber;

    private GameObject activatedBy;
    public int CubeNumber { get { return this.cubeNumber; } }


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

    public void Activate(Component sender, int objectNumber, string targetName, object data)
    {
        if (isActive || !CheckLaserCubeNmber(objectNumber) || targetName != "LaserCube")
        {
            return;
        }
        activatedBy = sender.gameObject;

        ToggleLaserCube(true);
    }

    public void Deactivate(Component sender, int objectNumber, string targetName, object data)
    {
        if (!isActive || activatedBy != sender.gameObject || !CheckLaserCubeNmber(objectNumber) || targetName != "LaserCube")
        {
            return;
        }

        activatedBy = null;
        ToggleLaserCube(false);
    }

    private void ToggleLaserCube(bool value)
    {
        if (value)
        {
            Debug.Log("Redirect Laser called!");
            isActive = true;
            ChangeCubeColor(activatedBy.GetComponent<Laser>().colorEnum);
            if (!lineRenderer.enabled)
            {
                lineRenderer.enabled = true;
                laserScript.enabled = true;
                laserScript.SetLaserColor(colorEnum);
                laserScript.direction = transform.forward;
                //Debug.Log("Direction: "+laserScript.direction);
                //Debug.Log("Rotation: "+transform.forward);
            }
        } else
        {
            laserScript.BlockLaserFromAll();
            laserScript.HideCollisionEffect();
            isActive = false;
            //ToggleLaserCube(isActive);
            lineRenderer.enabled = false;
            laserScript.enabled = false;

            Debug.Log($"laser cube stopped redirecting");
        }
    }

    private bool CheckLaserCubeNmber(int laserCubeNumber)
    {
        return this.cubeNumber == laserCubeNumber;
    }
}
