using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTargetingSystem : MonoBehaviour
{
    [SerializeField]
    private bool _canBeTargeted;
   
    public void SetCanBeTargeted(bool CanItBe)
    {
        _canBeTargeted = CanItBe;
    }
    public bool GetCanBeTargeted()
    {
        return _canBeTargeted;
    }

}
