using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserReceiver : MonoBehaviour, ILaserInteractable
{
    public int ReceiverNumber { get { return receiverNumber; } }
    [SerializeField] private int receiverNumber;
    [SerializeField] public LaserColors selectedLaserColor;
    private Color color;

    [SerializeField]
    private float seconds;

    // this is the list of gameobjects that will be activated by the receiver.
    // they must implement the IActivable interface
    public List<GameObject> activateList;

    private bool activated;
    private Coroutine activationRoutine;
    
    // Start is called before the first frame update
    void Start()
    {
        color = (Color) Colors.GetLaserColor(selectedLaserColor);
    }


    public void LaserCollide(Laser sender)
    {
        if (sender.colorEnum != selectedLaserColor) return;
        if (activated || activationRoutine != null) return;
        
        activated = true;
        activationRoutine = StartCoroutine(Wait(sender));
    }

    public void LaserExit(Laser sender)
    {
        if (sender.colorEnum != selectedLaserColor) return;
        if (!activated) return;
        if (activationRoutine != null)
        {
            StopCoroutine(activationRoutine);
            activationRoutine = null;
            Debug.Log("Stopped activation");
        }
        activated = false;
        activateList.ForEach(c => c.GetComponent<IActivable>()?.Deactivate(this));
        Debug.Log("Now inactive");
    }

    private IEnumerator Wait(Laser sender)
    {
        Debug.Log("Started laser wait");
        // use ? to check if they have an IActivable component
        yield return new WaitForSeconds(seconds);
        activateList.ForEach(c => c.GetComponent<IActivable>()?.Activate(this));
        activationRoutine = null;
        Debug.Log("Ended laser wait");
    }
}
