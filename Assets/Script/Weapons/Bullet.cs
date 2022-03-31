using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    public float bulletSpeed;
    [SerializeField]
    public float bulletDamage;

    [SerializeField]
    PhotonView myPV;

    public Rigidbody myRB;

    public Vector3 offset;

    void Awake()
    {
        myRB = this.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        //if Bullet is spawned in it automaticly adds forward force with offset
        myRB.AddForce((transform.forward+offset)*bulletSpeed);
        
        //Bullets are Destroyed after 10 seconds if nothing is hit
        Destroy(gameObject,10f);
    }

    //sends hit info to Player hit by Bullet
    private void SendPlayerHitInfo(Player hitPlayer)
    {
        object[] data = new object[] {myPV.Owner, hitPlayer, bulletDamage};
        RaiseEventOptions reo = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(RaiseEventCodes.PlayerHitInfo, data, reo, SendOptions.SendReliable);
    }

    private void OnTriggerEnter(Collider col)
    {
        //is the hit Object a player?
        if(col.gameObject.tag == "Player")
        {
            //setting Refrence to hit Player
            Player otherPlayer = col.gameObject.GetComponent<PhotonView>().Owner;

            //did i hit a other player?
            if (otherPlayer != myPV.Owner)
            {
                //send hit info to other player
                SendPlayerHitInfo(otherPlayer);
            }
        }
        if (col.gameObject.tag!="Fence")
        { 
            //destroys bullet after hitting something
            Destroy(this.gameObject);
        }

    }
}
