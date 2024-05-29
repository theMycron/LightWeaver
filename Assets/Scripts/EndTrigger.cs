using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EndTrigger : MonoBehaviour
{
    private bool levelComplete = false;
    public GameEvent levelDone;
    private void OnTriggerEnter(Collider other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController == null || levelComplete) return;
        Debug.Log("Level complete!");
        levelComplete = true;
        // raise event to end level
        levelDone.Raise(this, 0, "Level", null);
    }
}
