using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicDummy : MonoBehaviour
{

    private NavMeshAgent agent;
    public GameObject target;
    public GameObject target2;
    private Rigidbody rigidbody;
    private bool isTargetOneActive;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rigidbody = GetComponent<Rigidbody>();
        agent.destination = target.transform.position;
        isTargetOneActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        

        
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.transform.tag);        
        SwitchTargets();
    }

    private void SwitchTargets()
    {
        if (isTargetOneActive)
        {
            agent.destination = target2.transform.position;
            isTargetOneActive = false;
        } else
        {
            agent.destination = target.transform.position;
            isTargetOneActive = true;
        }
    }
    
}
