using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmgPickUp : PickUps
{
    private void Awake()
    {
        
    }

    void Update()
    {
        delayTimer += Time.deltaTime;

        //ends the delay if delay is set and the delay is over
        if (!myRenderer.enabled && delayTimer > delayTime)
        {
            //ending delay by making Object visible again
            myRenderer.enabled = true;
        }
        Move();
    }
}
