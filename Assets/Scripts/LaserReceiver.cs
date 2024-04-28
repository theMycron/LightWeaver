using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserReceiver : MonoBehaviour
{
    public int GateNumber { get { return gateNumber; } }
    [Header("Gate Attached")]
    [SerializeField]
    private int gateNumber;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LaserCollided(Component sender, object data, int gateNumber)
    {
        Debug.Log("I have been hitten By Laser From unknown!" + sender);
    }

    public void LaserBlocked(Component sender, object data, int gateNumber)
    {
        Debug.Log("Laser Blocked!" + sender + " " + gateNumber);
    }
}
