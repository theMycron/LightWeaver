using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDisable
{
    void Deactivate(Component sender, int objectNumber, string targetName, object data);
}
