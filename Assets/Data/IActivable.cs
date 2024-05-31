using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActivable
{

    void Activate(Component sender);
    void Deactivate(Component sender);

}
