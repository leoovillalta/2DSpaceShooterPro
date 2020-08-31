using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMissileTargetable
{
    bool CanBeTargeted();
    void LockedOn();
}
