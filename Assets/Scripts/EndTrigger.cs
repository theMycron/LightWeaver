using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTrigger : MonoBehaviour
{
    private bool levelComplete = false;
    private void OnTriggerEnter(Collider other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController == null || levelComplete) return;
        Debug.Log("Level complete!");
        levelComplete = true;
        // raise event to end level
    }
}
