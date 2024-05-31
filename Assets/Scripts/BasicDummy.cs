using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class BasicDummy : MonoBehaviour
{

    private NavMeshAgent agent;
    private Rigidbody rigidbody;

    [SerializeField]
    private int dummyNumber;

    public int DummyNumber { get { return dummyNumber; } }

    PlayerController controller;

    Vector2 direction;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<PlayerController>();
        //agent = GetComponent<NavMeshAgent>();
        rigidbody = GetComponent<Rigidbody>();
        //agent.destination = target.transform.position;
        direction = new Vector3(transform.forward.x ,0f, transform.forward.z);
        direction = Quaternion.Euler(0, -controller.mainCamera.transform.eulerAngles.y, 0) * direction;
        
    }


    private void FixedUpdate()
    {
        controller.moveDirection = direction;

        animator.SetInteger("BaseState", (int)PlayerController.AnimationState.walking);
        //rigidbody.AddForce(direction * 100);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.transform.tag);

        direction = -1 * direction;

    }

    public void LaserCollide(Laser sender)
    {
        switch (sender.colorEnum)
        {
            case LaserColors.red:
                Destroy(gameObject, 0.1f); break;
            case LaserColors.blue:
                //animator.StopPlayback();
                animator.enabled = false;
                controller.enabled = false;
                break;
        }
    }

    public void LaserExit(Laser sender)
    {
        // only for freeze
        if (sender.colorEnum == LaserColors.red) return;
        animator.enabled = true;
        controller.enabled = true;
        Debug.Log("unFreeze logic");
    }



}
