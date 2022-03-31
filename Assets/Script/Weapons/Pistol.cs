using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon
{
    
    void Start()
    {
        //setting WeaponType
        myType = WType.Pistol;

        shotFrequency = 0.5f;

        //max sprayInaccuracy
        maxOffset = 0.3f;

        //how fast the offset is apllied
        offsetMultiplier = 1;

        
    }

    
    void Update()
    {  
        //counts Timer until next available shot
        shotTimer += Time.deltaTime;
       
    }

    public override void Shoot()
    {
        //shoots if shotTimer exceeded shotFrequency
        if (CheckTimer(shotTimer) == true)
        {
            //Spawns bulletPrefab at bulletSpawn and shoots it forward
            SpawnBullet(bulletPrefab, bulletSpawn);

            //resets shotTimer
            shotTimer = 0;

            //plays the shot sound
            myAS.Play();

            object[] data = new object[] { PhotonNetwork.LocalPlayer, myType };
            RaiseEventOptions reo = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent(RaiseEventCodes.PlayWeaponSound, data, reo, SendOptions.SendReliable);
        }
    }

}
