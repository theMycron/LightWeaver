using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;

[CreateAssetMenu(menuName="GameEvent")]
public class GameEvent : ScriptableObject
{

    public List<GameEventListener> listeners = new List<GameEventListener>();

    // Raise Event through different methods signatures
    public void Raise(Component sender, object data)
    {
        foreach (GameEventListener listener in listeners)
        {
            listener.onEventRaised(sender, data);
        }
    }

    // Manage Listeners

    public void RegisterListener(GameEventListener listener)
    {
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }

    public void UnregisterListener(GameEventListener listener)
    {
        if (listeners.Contains(listener))
        {
            listeners.Remove(listener);
        }
    }
}
