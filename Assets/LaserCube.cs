using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCube : MonoBehaviour
{
    [SerializeField]
    private Material redMaterial;

    [SerializeField] 
    private Material blueMaterial;

    private LineRenderer lineRenderer;

    [SerializeField]
    private Transform startPoint;
    
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, startPoint.localPosition);
        //lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = 0.4059853f;
        lineRenderer.endWidth = 0.4059853f;
        lineRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RedirectLaser(Component sender, object data, int t)
    {
        Debug.Log("Redirect Laser called!");
        ChangeInnerSphereColor(sender);
        if (!lineRenderer.enabled)
        {
            lineRenderer.enabled = true;
        }

        //Debug.DrawLine(startPoint.position, new Vector3(0f, 0f, 15f), Color.red);
    }

    private void ChangeInnerSphereColor(Component sender)
    {
        
        if (sender.tag.StartsWith("Blue"))
        {
            GameObject.Find("InnerSphere").gameObject.GetComponent<MeshRenderer>().material = blueMaterial;
            lineRenderer.material = blueMaterial;
        } else
        {
            GameObject.Find("InnerSphere").gameObject.GetComponent<MeshRenderer>().material = redMaterial;
            lineRenderer.material = redMaterial;
        }
        
    }
}
