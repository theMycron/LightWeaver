using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILaserInteractable
{
    public void LaserCollide(Laser sender);
    public void LaserExit();
}
