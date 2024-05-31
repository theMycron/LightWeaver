using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public record SaveProfile
{
    // any game data that needs to be saved
    public int lastUnlockedLevel;
    public int collectiblesCollected;
}
