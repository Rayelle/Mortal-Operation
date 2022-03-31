using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;
using ExitGames.Client.Photon;

//all the Types a Weapon could be
public enum WType
{
    Pistol,
    SMG,
    Sniper
}

public class Weapon : MonoBehaviour, IOnEventCallback
{
    #region Variables

    //Refrence to Bullet that should be shot
    [SerializeField]
    public string bulletPrefab;

    protected float shotFrequency, shotTimer, offsetMultiplier,maxOffset,ammo;

   [SerializeField]
    protected float startingAmmo;

    protected WType myType;

    //Spawnpoint of fired Bullets
    [SerializeField]
    protected Transform bulletSpawn;

    [SerializeField]
    Transform bulletParent;
    
    public Vector3 offset;

    [SerializeField]
    protected AudioSource myAS;

    [SerializeField]
    PhotonView myPV;

    [SerializeField]
    AudioClip myShotSound;

    public float OffsetMultiplier { get => offsetMultiplier; set => offsetMultiplier = value; }
    public float MaxOffset { get => maxOffset; set => maxOffset = value; }
    public WType MyType { get => myType; set => myType = value; }
    public float Ammo { get => ammo; set => ammo = value; }
    public float StartingAmmo { get => startingAmmo; set => startingAmmo = value; }

    #endregion

    private void OnEnable()
    {
        //this class tracks events
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public virtual void Shoot()
    {
        //has shotTimer exceeded the shotFrequency?, is Ammo left?
        if (CheckTimer(shotTimer) == true && ammo > 0)
        {
            //Spawns and shoots Bullet
            SpawnBullet(bulletPrefab, bulletSpawn);

            //counts down ammo
            ammo -= 1;

            //reset shot Timer
            shotTimer = 0;

            //play weapon sound
            myAS.Play();

            //all other clients receive event to play shot sound
            object[] data = new object[] { PhotonNetwork.LocalPlayer, myType };
            RaiseEventOptions reo = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent(RaiseEventCodes.PlayWeaponSound, data, reo, SendOptions.SendReliable);
        }


    }

    #region Timer
   
    public virtual bool CheckTimer(float shotTimer)
    {
        if (shotTimer >= shotFrequency) return true;
        else return false;

    }
    #endregion

    //Spawns Bullet
    public virtual GameObject SpawnBullet(string bulletPrefab,Transform bulletSpawn)
    {
        //instantiates Bullet in Game Lobby
        GameObject bullet = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", bulletPrefab), bulletSpawn.position, this.transform.rotation);
        
        //sets Parent for shot Bullets
        bullet.transform.SetParent(bulletParent);

        //tells Bullet the current shooting offset
        bullet.GetComponent<Bullet>().offset = offset;
        return bullet;
    }


    public void OnEvent(EventData photonEvent)
    {
        byte raiseEventCode = photonEvent.Code;


        //other clients receive shooting sound via event
        if (raiseEventCode == RaiseEventCodes.PlayWeaponSound)
        {
            object[] data = (object[])photonEvent.CustomData;
            if ((Player)data[0] == myPV.Owner )
            {
                if ((WType)data[1] == myType)
                {
                    myAS.clip = myShotSound;
                    myAS.Play();
                }
            }
        }
    }
}
