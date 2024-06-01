using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class BasicDummy : MonoBehaviour
{

    enum Directions
    {
        North,
        South,
        East,
        West
    }

    [SerializeField]
    private int dummyNumber;

    public int DummyNumber { get { return dummyNumber; } }

    PlayerController controller;

    Vector2 direction;

    private Animator animator;

    [Header("Surronded")]
    [SerializeField] GameObject shoulderLevel;
    [SerializeField] GameObject kneesLevel;

    [Header("Direction")]
    [SerializeField] private Directions selectedDirection; 

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(-transform.forward);
        Debug.Log(-transform.right);
        animator = GetComponent<Animator>();
        controller = GetComponent<PlayerController>();

        //direction = -transform.forward;
        SetDirection();
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

    private void SetDirection()
    {
        switch (selectedDirection)
        {
            case Directions.North:
                direction = -transform.forward; break;
            case Directions.South:
                direction = new Vector3(10f, 0f, 10f); break;
            case Directions.East:
                direction = transform.right; break;
            case Directions.West:
                direction = -transform.right; break;
        }
    }

}
