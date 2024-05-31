using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorButton : MonoBehaviour

{

    private Animator animator;
    private Animator gateAnimator;

    // this is the list of gameobjects that will be activated by the receiver.
    // they must implement the IActivable interface
    public List<GameObject> activateList;

    //private GameObject gate;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInParent<Animator>();
        //gateAnimator = gate.GetComponent<Animator>();
    }


    private void ToggleButton(bool toggle)
    {
        animator.SetBool("isObjectOver", toggle);
        Debug.Log("Floor button pressed");
        if (toggle)
            activateList.ForEach(c => c.GetComponent<IActivable>()?.Activate(this));
        else
            activateList.ForEach(c => c.GetComponent<IActivable>()?.Deactivate(this));
        
    }

    private void OnTriggerEnter(Collider other)
    {
        ToggleButton(true);
    }

    private void OnTriggerExit(Collider other)
    {
        ToggleButton(false);
    }
}
