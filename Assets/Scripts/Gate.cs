using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour, IActivable
{
    private Animator animator;

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
}
