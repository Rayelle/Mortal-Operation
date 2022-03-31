using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerData : MonoBehaviour, IOnEventCallback
{
    FollowCamera myCamera;

    [SerializeField]
    float hp;
    float maxHP;

    [SerializeField]
    PhotonView myPV;

    Slider myHealthBar;


    [SerializeField]
    GameObject myMeshParent,myLaserParent;

    [SerializeField]
    Collider myCol;

    [SerializeField]
    Rigidbody myRB;

    bool isDead = false;

    public Slider MyHealthBar { set => myHealthBar = value; }
    public FollowCamera MyCamera { set => myCamera = value; }
    public PhotonView MyPV { get => myPV; }
    public GameObject MyLaserParent { get => myLaserParent; }

    private void Awake()
    {
        //remember maximum hitpoints for resetting players
        maxHP = hp;
    }

    private void OnEnable()
    {
        //this class tracks events
        PhotonNetwork.AddCallbackTarget(this);

    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    private void Start()
    {
        if(MyPV.IsMine)
            myLaserParent.SetActive(true);
    }
    /// <summary>
    /// Take an amount of damage, if it exceedes current hp players will die
    /// </summary>
    /// <param name="damageAmount"></param>
    /// <param name="killer"></param>
    public void TakeDamage(float damageAmount, Player killer)
    {

            if (damageAmount >= hp)
            {
                hp = 0;
                myHealthBar.value = hp;
                //killer is passed down into death-method for event information
                if(!isDead)
                    PlayerDead(killer);
            }
            else
            {
                hp -= damageAmount;
                myHealthBar.value = hp;
            }
        
    }

    /// <summary>
    /// Disable players hitbox and mesh for all clients
    /// send event for score information
    /// </summary>
    /// <param name="killer"></param>
    private void PlayerDead(Player killer)
    {
        //adjust followCamera to be in spectator mode
        myCamera.SpectatorModeOn = true;
        myPV.RPC("RemoteDead", RpcTarget.All);

        isDead = true;

        object[] data = new object[] { myPV.Owner };
        RaiseEventOptions reo = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(RaiseEventCodes.LooseControl, data, reo, SendOptions.SendReliable);

        data = new object[] { myPV.Owner, killer };
        reo = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(RaiseEventCodes.PlayerDeath, data, reo, SendOptions.SendReliable);
    }

    /// <summary>
    /// sets player to spectator mode and disables hitbox and mesh for all clients
    /// </summary>
    private void PlayerSpectating()
    {
        myLaserParent.SetActive(false);
        myCamera.SpectatorModeOn = true;
        myPV.RPC("RemoteDead", RpcTarget.All);
    }

    /// <summary>
    /// disables hitbox and mesh, resets velocity so players dont fall through the map while inactive
    /// </summary>
    [PunRPC]
    private void RemoteDead()
    {
        myCol.enabled = false;
        myRB.useGravity = false;
        myRB.velocity = Vector3.zero;
        myMeshParent.SetActive(false);
        myLaserParent.SetActive(false);
    }

    /// <summary>
    /// enables hitbox and mesh
    /// resets velocity so players dont move at new spawnpoint
    /// set position to spawnpoint and resets camera above player
    /// </summary>
    /// <param name="spawnPosition"></param>
    public void ResetPlayer(Vector3 spawnPosition)
    {
        isDead = false;
        myCol.enabled = true;
        myRB.useGravity = true;
        myRB.velocity = Vector3.zero;
        myMeshParent.SetActive(true);


        transform.position = spawnPosition;

        hp = maxHP;

        if (myPV.IsMine)
        {
            myHealthBar.value = hp;

            myCamera.transform.position = this.transform.position + myCamera.DistanceToPlayer;
            myCamera.SpectatorModeOn = false;
            myLaserParent.SetActive(true);
        }

    }

    public void OnEvent(EventData photonEvent)
    {
        byte raiseEventCode = photonEvent.Code;

        //players are put into spectator mode if they join a running round
        if (raiseEventCode == RaiseEventCodes.PlayerSpectate )
        {
            object[] data = (object[])photonEvent.CustomData;
            if ((Player)data[0] == PhotonNetwork.LocalPlayer)
            {
                PlayerSpectating();
            }
        }

        //players take damage through event
        if (raiseEventCode == RaiseEventCodes.PlayerHitInfo && myPV.IsMine)
        {

            object[] data = (object[]) photonEvent.CustomData;
            if((Player)data[1] == PhotonNetwork.LocalPlayer)
            {
                TakeDamage((float)data[2],(Player)data[0]);
            }
        }

        //players respawn through event
        if(raiseEventCode == RaiseEventCodes.PlayerRespawn )
        {
            object[] data = (object[])photonEvent.CustomData;
            if ((Player)data[0] == myPV.Owner)
            {
                ResetPlayer((Vector3)data[1]);
            }
        }
    }
}
