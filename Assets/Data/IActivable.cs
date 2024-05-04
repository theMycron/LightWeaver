using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActivable
{

    void Activate(Component sender, int objectNumber, string targetName, object data);

}
