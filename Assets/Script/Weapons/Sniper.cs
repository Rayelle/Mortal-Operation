using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : Weapon
{

    void Start()
    {
        //setting WeaponType
        myType = WType.Sniper;

        shotFrequency = 0.8f;

        //max sprayInaccuracy
        maxOffset = 0f;

        //how fast the offset is apllied
        offsetMultiplier = 0;

        //maximum Ammo / starting Ammo of this Weapon
        startingAmmo = 15;
    }


    void Update()
    {
        shotTimer += Time.deltaTime;
    }


}

