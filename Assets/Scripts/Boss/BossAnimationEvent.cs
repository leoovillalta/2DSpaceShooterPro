using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimationEvent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void PlayerActorDodge()
    {
        Animator anim = transform.Find("PlayerActor").GetComponent<Animator>();
        anim.SetTrigger("EvasiveManeuverRight");
    }
}
