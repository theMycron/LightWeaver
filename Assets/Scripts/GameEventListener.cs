using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// to see it in the inspector
[System.Serializable]
public class CustomGameEvent : UnityEvent<Component, object, int> {}

public class GameEventListener : MonoBehaviour
{

    [Tooltip("Event to register with.")]
    public GameEvent gameEvent;

    [Tooltip("Response to invoke when Event with GameEvent is Raiser")]
    //public UnityEvent response;
    public CustomGameEvent response;

    private void OnEnable()
    {
        gameEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        gameEvent.UnregisterListener(this);
    }

    public void onEventRaised(Component sender, object data, int gateNumber)
    {
        response.Invoke(sender, data, gateNumber);
    }
}
