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

    private ParticleSystem particleSystem;

    // Start is called before the first frame update
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        ToggleEmitter(isActive);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Activate(Component sender, int objectNumber, string targetName, object data)
    {
        if (!isActive && CheckEmitterNumber(objectNumber) && targetName == "Emitter")
        {
            isActive = true;
            ToggleEmitter(isActive);
            Debug.Log("Activate laser emitter!");
        }
        
    }

    public void Deactivate(Component sender, int objectNumber, string targetName, object data)
    {
        if (CheckEmitterNumber(objectNumber) && targetName == "Emitter")
        {
            isActive = false;
            ToggleEmitter(isActive);
            Debug.Log("Deactivate laser emitter!");
        }
        
    }

    private void ToggleEmitter(bool isActive)
    {
        Laser laserScript = gameObject.GetComponent<Laser>();
        LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
        laserScript.enabled = isActive;
        lineRenderer.enabled = isActive;
        if (isActive)
        {
            particleSystem.Play();
        } else
        {
            particleSystem.Stop();
        }
    }

    private bool CheckEmitterNumber(int emitterNumber)
    {
        return this.number == emitterNumber;
    }

}
