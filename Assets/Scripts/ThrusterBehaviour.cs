using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrusterBehaviour : MonoBehaviour
{
    private Animator _anim;
    private bool _thrustersActivated=false;

    // Start is called before the first frame update
    void Start()
    {
        _anim = transform.GetComponent<Animator>();
    }

    public void IncreasedRateTrusters(bool activated)
    {
        _thrustersActivated = activated;
        if (_thrustersActivated)
        {
            _anim.speed = 10f;
            _anim.SetBool("FullPowerThrusters", true);
        }
        else
        {
            _anim.speed = 1f;
            _anim.SetBool("FullPowerThrusters", false);
        }
    }
}
