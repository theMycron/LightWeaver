using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorButton : MonoBehaviour

{

    private Animator animator;
    private Animator gateAnimator;

    [Header("Gates Attached")]
    [SerializeField]
    //private GameObject gate;

    [Header("Events")]
    public GameEvent onFloorButtonPressed;
    public GameEvent onFloorButtonDePressed;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInParent<Animator>();
        //gateAnimator = gate.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "cube")
        {
            //gateAnimator.SetBool("isOpened", true);
            animator.SetBool("isObjectOver", true);
        }

        if (other.gameObject.tag.StartsWith("Robot"))
        {
            animator.SetBool("isObjectOver", true);
            //gateAnimator.SetBool("isOpened", true);
        }

        onFloorButtonPressed.Raise(this, animator.GetBool("isObjectOver"));
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "cube")
        {
            //gateAnimator.SetBool("isOpened", false);
            animator.SetBool("isObjectOver", false);
        }

        if (other.gameObject.tag.StartsWith("Robot"))
        {
            animator.SetBool("isObjectOver", false);
            //gateAnimator.SetBool("isOpened", false);
        }

        onFloorButtonDePressed.Raise(this, animator.GetBool("isObjectOver"));
    }
}
