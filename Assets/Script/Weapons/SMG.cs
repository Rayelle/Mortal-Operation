using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMG : Weapon
{
 
    
    void Start()
    {
        //setting WeaponType
        myType = WType.SMG;

        shotFrequency = 0.1f;

        //max sprayInaccuracy
        maxOffset = 0.3f;

        //how fast the offset is apllied
        offsetMultiplier = 1;

        //maximum Ammo / starting Ammo of this Weapon
        startingAmmo = 50;
    }


    void Update()
    {
        shotTimer += Time.deltaTime;
    }


}

