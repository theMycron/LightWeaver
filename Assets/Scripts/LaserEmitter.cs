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
    public LaserColors selectedLaserColor;

    private Color color;

    private ParticleSystem _particleSystem;
    private Laser laserScript;
    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        color = (Color)Colors.GetLaserColor(selectedLaserColor);
        laserScript = gameObject.GetComponent<Laser>();
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        _particleSystem = GetComponent<ParticleSystem>();
        var main = _particleSystem.main;
        main.startColor = color;
        laserScript.color = color;
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
        if (isActive && CheckEmitterNumber(objectNumber) && targetName == "Emitter")
        {
            isActive = false;
            ToggleEmitter(isActive);
            Debug.Log("Deactivate laser emitter!");
        }
    }

    private void ToggleEmitter(bool isActive)
    {
       
        if (isActive)
        {
            _particleSystem.Play();
        } else
        {
            laserScript.BlockLaserFromAll();
            _particleSystem.Stop();
            //particleSystem.gameObject.SetActive(false);
        }
        lineRenderer.enabled = isActive;
        Debug.Log($"laserscript color: {laserScript.color}");
        laserScript.enabled = isActive;
        laserScript.SetLaserColor(selectedLaserColor);
    }

    private bool CheckEmitterNumber(int emitterNumber)
    {
        return this.number == emitterNumber;
    }

}
