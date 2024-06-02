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

    private List<GameObject> activatedByList;
    private Coroutine activationRoutine;
    
    // Start is called before the first frame update
    void Start()
    {
        color = (Color) Colors.GetLaserColor(selectedLaserColor);
        activatedByList = new List<GameObject>();
    }


    public void LaserCollide(Laser sender)
    {
        if (sender.colorEnum != selectedLaserColor) return;
        if (activationRoutine != null) return;
        if (activatedByList.Contains(sender.gameObject)) return;
        activatedByList.Add(sender.gameObject);
        if (activatedByList.Count > 1) return;
        activationRoutine = StartCoroutine(Wait(sender));
    }

    public void LaserExit(Laser sender)
    {
        if (sender.colorEnum != selectedLaserColor) return;
        if (activatedByList.Contains(sender.gameObject))
            activatedByList.Remove(sender.gameObject);
        else return;

        if (activatedByList.Count > 0) return;
        if (activationRoutine != null)
        {
            StopCoroutine(activationRoutine);
            activationRoutine = null;
            Debug.Log("Stopped activation");
        }
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
