using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour, IActivable, IDisable
{
    private Animator animator;
    // Start is called before the first frame update
    [SerializeField]
    private int gateNumber;

    private bool hasLaserDetectedBefore;
    private bool hasLaserBlockedBefore;

    [SerializeField]
    private int activationsRequired;
    void Start()
    {
        animator = GetComponent<Animator>();
        hasLaserDetectedBefore = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public void ManageGate(Component sender, object data, int gateNumber)
    //{
    //    if (data is bool)
    //    {
    //        bool isObjectOver = (bool)data;
    //        Debug.Log("Event recevied" + isObjectOver + " " + gateNumber);
    //        if (isObjectOver)
    //        {
    //            OpenGate();
    //        } else
    //        {
    //            CloseGate();
    //        }
    //    }
        
    //}

    private bool CheckGateNumber(int gateNumber)
    {
        return this.gateNumber == gateNumber;
    }

    public void Activate(Component sender, int objectNumber, string targetName, object data)
    {

        if (CheckGateNumber(objectNumber) && targetName == "Gate")
        {
            if (sender.tag.EndsWith("Emitter"))
            {
                if (!hasLaserDetectedBefore)
                {
                    activationsRequired--;
                    hasLaserDetectedBefore = true;
                    hasLaserBlockedBefore = false;
                }

            }
            else
            {
                activationsRequired--;
            }

            if (activationsRequired == 0)
            {
                animator.SetBool("isOpened", true);
            }
        }
    }

    public void Deactivate(Component sender, int objectNumber, string targetName, object data)
    {
        if (CheckGateNumber(objectNumber) && targetName == "Gate")
        {
            if (sender.tag.EndsWith("Emitter"))
            {
                if (!hasLaserBlockedBefore)
                {
                    hasLaserBlockedBefore = true;
                    activationsRequired++;
                }
                hasLaserDetectedBefore = false;
            }
            else
            {
                activationsRequired++;
            }

            if (activationsRequired != 0)
            {
                animator.SetBool("isOpened", false);
            }

        }
    }

    //private void ResetGateParameters()
    //{
    //    hasLaserDetectedBefore = false;
    //}
}
