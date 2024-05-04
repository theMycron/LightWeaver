using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserReceiver : MonoBehaviour
{
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
        Debug.Log("I have been hitten By Laser From unknown!" + sender);
    }

    public void LaserBlocked(Component sender, int objectNumber, string targetName, object data)
    {
        Debug.Log("Laser Blocked!" + sender + " " + gateNumber);
    }
}
