using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUps : MonoBehaviour, IOnEventCallback
{
    [SerializeField]
    protected float delayTime;
    //Time until Weapon is ready to be picked up again

    [SerializeField]
    protected PhotonView mypv;

    [SerializeField]
    protected WType pickUp;

    //Respawn dely of the pickUp
    protected float delayTimer;

    [SerializeField]
    protected Renderer myRenderer;

    //the degrees the pickUp roates per second
    [SerializeField]
    protected float degreesPerSecond;

    //the amplitude of sin curve thats used to float up and down
    [SerializeField]
    protected float amplitude;

    //the frequency of sin curve thats used to float up and down
    [SerializeField]
    protected float frequency;

    [SerializeField]
    AudioSource myAS;

    //Position Storage Variables
    private Vector3 posOffset = new Vector3();
    private Vector3 tempPos = new Vector3();

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void Start()
    {
        //sets pickUp delay for all players in Lobby
        mypv.RPC("SetDelay", RpcTarget.All);
        //storing starting position and rotation
        posOffset = transform.position;
    }

    void Update()
    {
        //counts up the delayTimer
        delayTimer += Time.deltaTime;

        //ends the delay if delay is set and the delay is over
        if (!myRenderer.enabled && delayTimer > delayTime)
        {
            //ending delay by making Object visible again
            myRenderer.enabled = true;
        }

        Move();
    }

    //moves the PickUp up and down and rotates on y axis
    protected void Move()
    {
        //rotates the set degrees per second around the y axis in World space
        transform.Rotate(new Vector3(0f,Time.deltaTime*degreesPerSecond,0f),Space.World);

        //floating up and down along a sin curve 
        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime*Mathf.PI * frequency)*amplitude;
        transform.position = tempPos;
    }
    protected void OnTriggerEnter(Collider col)
    {
        // only accessible if not on delay and a player collides
        if (myRenderer.enabled && col.gameObject.tag == "Player")
        {
            //refrence to the WeaponController of the Player
            WeaponController myController= col.gameObject.GetComponentInChildren<WeaponController>();

            //if PickUp is a SMG tell WC to pick it up
            if (pickUp==WType.SMG)
            {
                myController.PickUpSMG();
            }
            //if PickUp is a Sniper tell WC to pick it up
            if (pickUp == WType.Sniper)
            {
                myController.PickUpSniper();
            }
            //play pickUp sound
            myAS.Play();

            //set the delay for the interaction
            mypv.RPC("SetDelay", RpcTarget.All);
        }
        
    }

    [PunRPC]
    protected virtual void SetDelay()
    {
        //reset timer and disable visibility
        delayTimer = 0f;
        myRenderer.enabled = false;
    }

    public void OnEvent(EventData photonEvent)
    {
        byte raiseEventCode = photonEvent.Code;

        //register player-death in playersAlive list
        if (raiseEventCode == RaiseEventCodes.TimerChange)
        {
            object[] data = (object[])photonEvent.CustomData;

            if ((string)data[0] == "3")
            {
                mypv.RPC("SetDelay", RpcTarget.All);
            }

        }
    }
}
