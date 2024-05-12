using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName="GameEvent")]
public class GameEvent : ScriptableObject
{

    public List<GameEventListener> listeners = new List<GameEventListener>();

    // Raise Event through different methods signatures

    //public void Raise(Component sender, object data)
    //{
    //    Raise(sender, data, -1);
    //}

    public void Raise(Component sender)
    {
        foreach (GameEventListener listener in listeners)
        {
            listener.onEventRaised(sender, -1, "", null);
        }
    }

    public void Raise(Component sender, int objectNumber, string targetName, object data)
    {
        foreach (GameEventListener listener in listeners)
        {
            listener.onEventRaised(sender, objectNumber, targetName, data);
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
