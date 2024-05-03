using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEmitter2 : MonoBehaviour, IActivable
{
    [SerializeField]
    private int number;

    [SerializeField]
    private bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        ToggleLaserScript(isActive);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {
        Debug.Log("Activate me please: laser emitter 2! " + isActive);
        isActive = true;
        ToggleLaserScript(isActive);
    }

    public void Disable()
    {
        Debug.Log("Disable me please: laser emitter 2! " + isActive);
        isActive = false;
        ToggleLaserScript(isActive);
    }

    private void ToggleLaserScript(bool isEnabled)
    {
        this.gameObject.GetComponent<Laser>().enabled = isEnabled;
    }
}
