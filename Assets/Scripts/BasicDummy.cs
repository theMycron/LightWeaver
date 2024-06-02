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

    [SerializeField] LayerMask collisionMask;

    [Header("Surronded")]
    [SerializeField] GameObject shoulderLevel;
    [SerializeField] GameObject kneesLevel;

    // Start is called before the first frame update
    void Start()
    {
        direction = new Vector2(transform.forward.x, transform.forward.z);
        // override default color
        texture.defaultColor = RobotTextureController.ROBOT_PURPLE;
        texture.defaultFaceColor = FaceColors.PURPLE;
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
    }

    private void FixedUpdate()
    {
        controller.moveDirection = direction;

        animator.SetInteger("BaseState", (int)PlayerController.AnimationState.walking);

        if (CheckIfHitted())
        {
            direction *= -1;
        }

    }

    public void LaserCollide(Laser sender)
    {
        switch (sender.colorEnum)
        {
            case LaserColors.red:
                Destroy(transform.parent.gameObject, 0.4f);
                animator.enabled = false;
                SetDummyMovement(false);
                break;
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
        Vector3 checkDirection = new Vector3(direction.x, 0, direction.y);

        RaycastHit kneeHit;
        RaycastHit shoulderHit;
        Debug.DrawRay(kneesLevel.transform.position, checkDirection * 5, Color.red, 1);
        Debug.DrawRay(kneesLevel.transform.position, -checkDirection * 5, Color.red, 1);

        Debug.DrawRay(shoulderLevel.transform.position, checkDirection * 5, Color.red, 1);
        Debug.DrawRay(shoulderLevel.transform.position, -checkDirection * 5, Color.red, 1);

        bool forwardKneeCheck = Physics.Raycast(kneesLevel.transform.position, checkDirection, out kneeHit, 5, collisionMask);
        bool backwardKneeCheck = Physics.Raycast(kneesLevel.transform.position, -checkDirection, out kneeHit, 5, collisionMask);
        bool forwardshoulderCheck = Physics.Raycast(shoulderLevel.transform.position, checkDirection, out shoulderHit, 5, collisionMask);
        bool backwardshoulderCheck = Physics.Raycast(shoulderLevel.transform.position, -checkDirection, out shoulderHit, 5, collisionMask);

        return (forwardKneeCheck && backwardKneeCheck) || (forwardshoulderCheck && backwardshoulderCheck);
    }

    private bool CheckIfHitted()
    {
        Vector3 checkDirection = new Vector3(direction.x, 0, direction.y);

        RaycastHit kneeHit;
        RaycastHit shoulderHit;

        //Debug.DrawRay(shoulderLevel.transform.position, -transform.forward * 2f, Color.red, 30f);
        //Debug.DrawRay(kneesLevel.transform.position, -transform.forward * 2f, Color.blue, 30f);
        Debug.DrawRay(shoulderLevel.transform.position, checkDirection * 2f, Color.blue, 1f);
        Debug.DrawRay(kneesLevel.transform.position, checkDirection * 2f, Color.blue, 1f);

        bool forwardKneeCheck = Physics.Raycast(kneesLevel.transform.position, checkDirection, out kneeHit, 2f, collisionMask);
        bool forwardshoulderCheck = Physics.Raycast(shoulderLevel.transform.position, checkDirection, out shoulderHit, 2f, collisionMask);

        return forwardKneeCheck || forwardshoulderCheck;
    }

    private void SetDummyMovement(bool isMoving)
    {
        controller.enabled = isMoving;
    }

}
