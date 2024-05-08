using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDirection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<Laser>().direction = -transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
