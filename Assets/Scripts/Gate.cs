using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ManageGate(Component sender, object data)
    {
        if (data is bool)
        {
            bool isObjectOver = (bool)data;
            Debug.Log("Event recevied" + isObjectOver);
            if (isObjectOver)
            {
                OpenGate();
            } else
            {
                CloseGate();
            }
        }
        
    }

    public void CloseGate()
    {
        animator.SetBool("isOpened", false);
    }

    public void OpenGate()
    {
        animator.SetBool("isOpened", true);
    }
}
