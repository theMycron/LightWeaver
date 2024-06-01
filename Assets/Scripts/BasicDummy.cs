using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class BasicDummy : MonoBehaviour
{

    [SerializeField]
    private PlayerController controller;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private RobotTextureController texture;

    Vector2 direction;

    [SerializeField] LayerMask turnMask;

    [Header("Surronded")]
    [SerializeField] GameObject shoulderLevel;
    [SerializeField] GameObject kneesLevel;

    // Start is called before the first frame update
    void Start()
    {
        direction = new Vector2(transform.forward.x, transform.forward.z);
        // override default color
        texture.defaultColor = RobotTextureController.ROBOT_PURPLE;
    }

    private void Update()
    {
        if(CheckIfSurronded())
        {
            animator.SetInteger("BaseState", (int)PlayerController.AnimationState.idle);
            SetDummyMovement(false);
        } else
        {
            SetDummyMovement(true);
        }

        if (CheckIfHitted())
        {
            direction *= -1;
        }
    }

    private void FixedUpdate()
    {
        controller.moveDirection = direction;

        animator.SetInteger("BaseState", (int)PlayerController.AnimationState.walking);
        
    }

    public void LaserCollide(Laser sender)
    {
        switch (sender.colorEnum)
        {
            case LaserColors.red:
                Destroy(gameObject, 0.1f); break;
            case LaserColors.blue:
                animator.enabled = false;
                SetDummyMovement(false);
                break;
        }
    }

    public void LaserExit(Laser sender)
    {
        // only for freeze
        if (sender.colorEnum == LaserColors.red) return;
        SetDummyMovement(true);
        animator.enabled = true;
    }

    private bool CheckIfSurronded()
    {
        RaycastHit kneeHit;
        RaycastHit shoulderHit;
        //Debug.DrawRay(kneesLevel.transform.position, transform.forward * 4, Color.red, 30f);
        //Debug.DrawRay(kneesLevel.transform.position, -transform.forward * 4, Color.blue, 30);

        //Debug.DrawRay(shoulderLevel.transform.position, transform.forward * 4, Color.red, 30f);
        //Debug.DrawRay(shoulderLevel.transform.position, -transform.forward * 4, Color.blue, 30);

        bool forwardKneeCheck = Physics.Raycast(kneesLevel.transform.position, transform.forward, out kneeHit, 5);
        bool backwardKneeCheck = Physics.Raycast(kneesLevel.transform.position, -transform.forward, out kneeHit, 5);
        bool forwardshoulderCheck = Physics.Raycast(shoulderLevel.transform.position, transform.forward, out shoulderHit, 5);
        bool backwardshoulderCheck = Physics.Raycast(shoulderLevel.transform.position, -transform.forward, out shoulderHit, 5);

        return forwardKneeCheck && backwardKneeCheck && forwardshoulderCheck && backwardshoulderCheck;
    }

    private bool CheckIfHitted()
    {
        RaycastHit kneeHit;
        RaycastHit shoulderHit;

        //Debug.DrawRay(shoulderLevel.transform.position, -transform.forward * 2f, Color.red, 30f);
        //Debug.DrawRay(kneesLevel.transform.position, -transform.forward * 2f, Color.blue, 30f);
        //Debug.DrawRay(shoulderLevel.transform.position, transform.forward * 2f, Color.red, 30f);
        //Debug.DrawRay(kneesLevel.transform.position, transform.forward * 2f, Color.blue, 30f);

        bool forwardKneeCheck = Physics.Raycast(kneesLevel.transform.position, transform.forward, out kneeHit, 2f, turnMask);
        bool forwardshoulderCheck = Physics.Raycast(shoulderLevel.transform.position, transform.forward, out shoulderHit, 2f, turnMask);

        return forwardKneeCheck && forwardshoulderCheck;
    }

    private void SetDummyMovement(bool isMoving)
    {
        controller.enabled = isMoving;
    }

}
