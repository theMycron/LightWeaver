using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
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

    public void CloseGate(Component sender, object data, int gateNumber)
    {
        //activationsRequired++;
        //if (CheckGateNumber(gateNumber) && activationsRequired != 0)
        //{
        //    animator.SetBool("isOpened", false);
        //}
        Debug.Log("close sender: " + sender);
        if (CheckGateNumber(gateNumber))
        {
            if (sender.tag.EndsWith("Emitter"))
            {
                if (!hasLaserBlockedBefore)
                {
                    hasLaserBlockedBefore = true;
                    activationsRequired++;
                }
                hasLaserDetectedBefore = false;
            } else
            {
                activationsRequired++;
            }
            
            if (activationsRequired != 0)
            {
                animator.SetBool("isOpened", false);
            }
            
        }
    }

    public void OpenGate(Component sender, object data, int gateNumber)
    {
        //activationsRequired--;
        //if (CheckGateNumber(gateNumber) && activationsRequired == 0)
        //{
        //    animator.SetBool("isOpened", true);
        //}
        if (CheckGateNumber(gateNumber))
        {
            if (sender.tag.EndsWith("Emitter"))
            {
                if (!hasLaserDetectedBefore)
                {
                    activationsRequired--;
                    hasLaserDetectedBefore = true;
                    hasLaserBlockedBefore = false;
                }
                
            } else
            {
                activationsRequired--;
            }
                
            if (activationsRequired == 0)
            {
                animator.SetBool("isOpened", true);
            }
        }
    }

    private bool CheckGateNumber(int gateNumber)
    {
        return this.gateNumber == gateNumber;
    }

    //private void ResetGateParameters()
    //{
    //    hasLaserDetectedBefore = false;
    //}
}
