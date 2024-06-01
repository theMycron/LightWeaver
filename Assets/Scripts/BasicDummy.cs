using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
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

    [Header("Surronded")]
    [SerializeField] GameObject shoulderLevel;
    [SerializeField] GameObject kneesLevel;

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

    private void Update()
    {
        if(CheckIfSurronded())
        {
            SetDummyMovement(false);
        } else
        {
            SetDummyMovement(true);
        }
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
                SetDummyMovement(false);
                break;
        }
    }

    public void LaserExit(Laser sender)
    {
        // only for freeze
        if (sender.colorEnum == LaserColors.red) return;
        SetDummyMovement(true);
        
    }

    private bool CheckIfSurronded()
    {
        RaycastHit kneehit;
        RaycastHit shoulderhit;
        //Debug.DrawRay(kneesLevel.transform.position, transform.forward * 4, Color.red, 30f);
        //Debug.DrawRay(kneesLevel.transform.position, -transform.forward * 4, Color.blue, 30);

        //Debug.DrawRay(shoulderLevel.transform.position, transform.forward * 4, Color.red, 30f);
        //Debug.DrawRay(shoulderLevel.transform.position, -transform.forward * 4, Color.blue, 30);

        bool forwardKneeCheck = Physics.Raycast(kneesLevel.transform.position, transform.forward, out kneehit, 5);
        bool backwardKneeCheck = Physics.Raycast(kneesLevel.transform.position, -transform.forward, out kneehit, 5);
        bool forwardshoulderCheck = Physics.Raycast(shoulderLevel.transform.position, transform.forward, out shoulderhit, 5);
        bool backwardshoulderCheck = Physics.Raycast(shoulderLevel.transform.position, -transform.forward, out shoulderhit, 5);

        return forwardKneeCheck && backwardKneeCheck && forwardshoulderCheck && backwardshoulderCheck;
    }

    private void SetDummyMovement(bool isMoving)
    {
        animator.enabled = isMoving;
        controller.enabled = isMoving;
    }

}
