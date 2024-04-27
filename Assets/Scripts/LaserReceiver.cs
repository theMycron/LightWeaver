using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserReceiver : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LaserCollided()
    {
        Debug.Log("I have been hitten By Laser From unknown!");
    }

    public void LaserBlocked()
    {
        Debug.Log("Laser Blocked!");
    }
}
