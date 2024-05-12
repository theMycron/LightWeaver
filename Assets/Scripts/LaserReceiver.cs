using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserReceiver : MonoBehaviour, ILaserInteractable
{
    public int ReceiverNumber { get { return receiverNumber; } }
    [SerializeField] private int receiverNumber;
    [SerializeField] public LaserColors selectedLaserColor;
    private Color color;

    // this is the list of gameobjects that will be activated by the receiver.
    // they must implement the IActivable interface
    public List<GameObject> activateList; 
    
    
    // Start is called before the first frame update
    void Start()
    {
        color = (Color) Colors.GetLaserColor(selectedLaserColor);
    }


    public void LaserCollide(Laser sender)
    {
        // use ? to check if they have an IActivable component
        if (sender.colorEnum != selectedLaserColor)
        {
            return;
        }
        activateList.ForEach(c => c.GetComponent<IActivable>()?.Activate(this));
    }

    public void LaserExit(Laser sender)
    {
        if (sender.colorEnum != selectedLaserColor)
        {
            return;
        }
        activateList.ForEach(c => c.GetComponent<IActivable>()?.Deactivate(this));
    }
}
