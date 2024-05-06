using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserReceiver : MonoBehaviour
{
    public int ReceiverNumber { get { return receiverNumber; } }
    public bool IsBlue { get { return isBlue; } }
    [SerializeField] private int receiverNumber;
    [SerializeField]  private bool isBlue;
    
    public int GateNumber { get { return gateNumber; } }
    [Header("Gate Attached")]
    [SerializeField]
    private int gateNumber;

    public int EmitterNumber { get { return emitterNumber; } }
    [Header("Emitter Attached")]
    [SerializeField]
    private int emitterNumber;

    public int RobotNumber { get { return robotNumber; } }
    [Header("Robot Attached")]
    [SerializeField]
    private int robotNumber;

    [Header("Game Events")]
    public GameEvent onLaserBlocked;
    public GameEvent onLaserCollided;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LaserCollided(Component sender, int objectNumber, string targetName, object data)
    {

        if (receiverNumber == objectNumber && targetName == "Receiver")
        {
            Debug.Log("receiver been hitten By Laser From " + sender);
            onLaserCollided.Raise(this, GateNumber, "Gate", null);
            onLaserCollided.Raise(this, RobotNumber, "Robot", null);
            onLaserCollided.Raise(this, EmitterNumber, "Emitter", null);
        }
    }

    public void LaserBlocked(Component sender, int objectNumber, string targetName, object data)
    {
        if (receiverNumber == objectNumber && targetName == "Receiver")
        {
            Debug.Log("Laser Blocked!" + sender + " " + gateNumber);
            onLaserBlocked.Raise(this, GateNumber, "Gate", null);
            //onLaserBlocked.Raise(this, RobotNumber, "Robot", null);
            onLaserBlocked.Raise(this, EmitterNumber, "Emitter", null);
        }
    }
}
