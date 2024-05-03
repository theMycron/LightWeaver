using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LaserEmitter : MonoBehaviour, IActivable, IDisable
{

    [SerializeField]
    private int number;

    [SerializeField]
    private bool isActive;

    // Start is called before the first frame update
    void Start()
    {
        ToggleEmitter(isActive);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Activate()
    {
        if (!isActive)
        {
            isActive = true;
            ToggleEmitter(isActive);
            Debug.Log("Activate laser emitter!");
        }
        
    }

    public void Deactivate()
    {
        isActive = false;
        ToggleEmitter(isActive);
        Debug.Log("Deactivate laser emitter!");
    }

    private void ToggleEmitter(bool isActive)
    {
        Laser laserScript = gameObject.GetComponent<Laser>();
        LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
        laserScript.enabled = isActive;
        lineRenderer.enabled = isActive;
    }

}
