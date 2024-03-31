using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEmitter : MonoBehaviour
{

    private LineRenderer lineRenderer;
    [SerializeField]
    private Transform startPoint;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(0, startPoint.position);
        RaycastHit hit;

        if(Physics.Raycast(transform.position, -transform.right, out hit))
        {

        } else
        {
            lineRenderer.SetPosition(1, -transform.right * 5000);
        }
    }
}
