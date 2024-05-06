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
    public Color color;

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
        lineRenderer.startWidth = 0.4059853f;
        lineRenderer.endWidth = 0.4059853f;
        lineRenderer.enabled = false;
        laserScript = GetComponent<Laser>();

        ChangeCubeColor(color);
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

    private void ChangeCubeColor(Color color)
    {
        
        if (color == Colors.LASER_BLUE)
        {
            GameObject.Find("InnerSphere").gameObject.GetComponent<MeshRenderer>().material = blueMaterial;
            lineRenderer.material = blueMaterial;
            isBlue = true;
        } else if (color == Colors.LASER_RED)
        {
            GameObject.Find("InnerSphere").gameObject.GetComponent<MeshRenderer>().material = redMaterial;
            lineRenderer.material = redMaterial;
        }
        this.color = color;
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
        laserScript.enabled = isActive;
        lineRenderer.enabled = isActive;
        if (value)
        {
            Debug.Log("Redirect Laser called!");
            isActive = true;
            ChangeCubeColor(activatedBy.GetComponent<Laser>().color);
            if (!lineRenderer.enabled)
            {
                lineRenderer.enabled = true;
                laserScript.enabled = true;
                laserScript.isBlue = isBlue;
                laserScript.color = color;
                laserScript.direction = transform.forward;
                //Debug.Log("Direction: "+laserScript.direction);
                //Debug.Log("Rotation: "+transform.forward);
            }
        } else
        {
            laserScript.BlockLaserFromAll();
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
