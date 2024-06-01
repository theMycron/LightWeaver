using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorButton : MonoBehaviour

{

    private Animator animator;
    private GameObject triggeredBy; // this is so that only one object can activate the button at a time
    private Coroutine activationRoutine;

    // this is the list of gameobjects that will be activated by the receiver.
    // they must implement the IActivable interface
    public List<GameObject> activateList;

    //private GameObject gate;
    [SerializeField]
    private float seconds;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInParent<Animator>();
    }


    private void ToggleButton(bool toggle)
    {
        animator.SetBool("isObjectOver", toggle);
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
        if (triggeredBy != null) return;
        if (activationRoutine != null) return;
        triggeredBy = other.gameObject;
        ToggleButton(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggeredBy != other.gameObject) return;
        if (activationRoutine != null)
        {
            StopCoroutine(activationRoutine);
            activationRoutine = null;
        }
        triggeredBy = null;
        ToggleButton(false);
    }

    private IEnumerator Wait(bool toggle)
    {
        if (toggle)
        {
            yield return new WaitForSeconds(seconds);
            activateList.ForEach(c => c.GetComponent<IActivable>()?.Activate(this));
        }
    }
}
