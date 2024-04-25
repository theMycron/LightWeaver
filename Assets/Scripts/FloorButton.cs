using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorButton : MonoBehaviour

{

    //private Animator animator;
    private Animator gateAnimator;

    [Header("Gates Attached")]
    [SerializeField]
    private GameObject gate;

    // Start is called before the first frame update
    void Start()
    {
        //animator = GetComponentInParent<Animator>();
        gateAnimator = gate.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "cube")
        {
            //animator.Play("Base Layer.Idle");
            gateAnimator.Play("Door1.Doors1|Open1", 1);
            gateAnimator.Play("Door2.Doors2|Open2", 2);
            gateAnimator.SetBool("isOpened", true);
            //animator.SetBool("isObjectOver", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "cube")
        {
            //animator.Play("Base Layer.Press|PressAction");
            gateAnimator.Play("Door1.Doors1|Close1", 1);
            gateAnimator.Play("Door2.Doors2|Close2", 2);
            gateAnimator.SetBool("isOpened", false);
            //animator.SetBool("isObjectOver", false);
        }
    }
}
