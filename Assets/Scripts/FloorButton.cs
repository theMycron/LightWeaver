using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FloorButton : MonoBehaviour

{

    private Animator animator;
    private List<GameObject> triggeredByList;
    private Coroutine activationRoutine;

    // this is the list of gameobjects that will be activated by the receiver.
    // they must implement the IActivable interface
    public List<GameObject> activateList;

    //private GameObject gate;
    [SerializeField]
    private float seconds;
    private bool active;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInParent<Animator>();
        triggeredByList = new List<GameObject>();
    }

    private void Update()
    {
        triggeredByList.RemoveAll(item => item == null);
        if (triggeredByList.Count <= 0)
        {
            if (activationRoutine != null)
            {
                StopCoroutine(activationRoutine);
                activationRoutine = null;
            }
            if (active)
                ToggleButton(false);
        }
    }

    private void ToggleButton(bool toggle)
    {
        animator.SetBool("isObjectOver", toggle);
        active = toggle;
        if (toggle)
        {
            activationRoutine = StartCoroutine(Wait(toggle));
        }
        else
        {
            activateList.ForEach(c => c.GetComponent<IActivable>()?.Deactivate(this));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        triggeredByList.Add(other.gameObject);
        if (activationRoutine != null) return;
        ToggleButton(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggeredByList.Contains(other.gameObject))
            triggeredByList.Remove(other.gameObject);
        else return;
        if (triggeredByList.Count > 0) return;
        if (activationRoutine != null)
        {
            StopCoroutine(activationRoutine);
            //stop riser sound
            EnvSFX.instance.StopRiserSound();
            activationRoutine = null;
        }
        ToggleButton(false);
    }

    private IEnumerator Wait(bool toggle)
    {
        if (toggle)
        {
            //play riser sound
            EnvSFX.instance.PlayChargingSFX(EnvSFX.instance.riserSound);
            yield return new WaitForSeconds(seconds);

            //stop riser sound and play activation sound
            EnvSFX.instance.StopRiserSound();
            EnvSFX.instance.PlayChargingSFX(EnvSFX.instance.activationSound);

            activateList.ForEach(c => c.GetComponent<IActivable>()?.Activate(this));
        }
    }
}
