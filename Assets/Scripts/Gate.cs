using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour, IActivable
{
    private Animator animator;
    // Start is called before the first frame update
    [SerializeField]
    private int gateNumber;

    private List<GameObject> activators = new List<GameObject>();

    [SerializeField]
    private int activationsRequired;

    private Collider[] doorColliders;

    private bool wasOpened = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        doorColliders = new Collider[2];
        doorColliders[0] = transform.GetChild(0).GetComponent<BoxCollider>();
        doorColliders[1] = transform.GetChild(1).GetComponent<BoxCollider>();
        animator.speed = 10f;
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

    private void ToggleGateOpen(bool open)
    {
        foreach (Collider collider in doorColliders)
        {
            collider.enabled = !open;
        }
        animator.SetBool("isOpened", open);
/*        if (open)
        {
            AudioManager.instance.PlayRobotSFX(AudioManager.instance.gateOpen);
            wasOpened = true;
        }
        else if (!open && wasOpened)
        {
            AudioManager.instance.PlayRobotSFX(AudioManager.instance.gateClosed);
            wasOpened = false;
        }*/
        animator.speed = 1f;
    }

    private bool CheckGateNumber(int gateNumber)
    {
        return this.gateNumber == gateNumber;
    }

    public void Activate(Component sender)
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
            ToggleGateOpen(true);
        }
    }

    public void Deactivate(Component sender)
    {
        //Debug.Log($"Trying to close gate {gateNumber}. objectnum: {objectNumber}. Sender tag: {sender.tag}");

        if (!activators.Contains(sender.gameObject))
        {
            return;
        }
        activators.Remove(sender.gameObject);
        activationsRequired++;

        if (activationsRequired != 0)
        {
            ToggleGateOpen(false);
        }
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
                ToggleGateOpen(true);
            }
        }
    }

    public void Deactivate(Component sender, int objectNumber, string targetName, object data)
    {
        if (CheckGateNumber(objectNumber) && targetName == "Gate")
        {
            //Debug.Log($"Trying to close gate {gateNumber}. objectnum: {objectNumber}. Sender tag: {sender.tag}");

            if (!activators.Contains(sender.gameObject))
            {
                return;
            }
            activators.Remove(sender.gameObject);
            activationsRequired++;

            if (activationsRequired != 0)
            {
                ToggleGateOpen(false);
            }

        }
    }
}
