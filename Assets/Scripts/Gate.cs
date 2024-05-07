using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour, IActivable, IDisable
{
    private Animator animator;
    // Start is called before the first frame update
    [SerializeField]
    private int gateNumber;

    private List<GameObject> activators = new List<GameObject>();

    [SerializeField]
    private int activationsRequired;
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

    private bool CheckGateNumber(int gateNumber)
    {
        return this.gateNumber == gateNumber;
    }

    public void Activate(Component sender, int objectNumber, string targetName, object data)
    {

        if (CheckGateNumber(objectNumber) && targetName == "Gate")
        {

            // if an object is already activating this gate, dont try to activate again
            if (activators.Contains(sender.gameObject))
            {
                return;
            }
            activators.Add(sender.gameObject);
            activationsRequired--;

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
            Debug.Log($"Trying to close gate {gateNumber}. objectnum: {objectNumber}. Sender tag: {sender.tag}");

            if (!activators.Contains(sender.gameObject))
            {
                return;
            }
            activators.Remove(sender.gameObject);
            activationsRequired++;

            if (activationsRequired != 0)
            {
                animator.SetBool("isOpened", false);
            }

        }
    }
}
