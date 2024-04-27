using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    private Animator animator;
    // Start is called before the first frame update
    [SerializeField]
    private int gateNumber;
    void Start()
    {
        animator = GetComponent<Animator>();
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
        if (CheckGateNumber(gateNumber))
        {
            animator.SetBool("isOpened", false);
        }
        
    }

    public void OpenGate(Component sender, object data, int gateNumber)
    {
        if (CheckGateNumber(gateNumber))
        {
            animator.SetBool("isOpened", true);
        }
        
    }

    private bool CheckGateNumber(int gateNumber)
    {
        return this.gateNumber == gateNumber;
    }
}
