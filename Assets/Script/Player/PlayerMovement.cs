using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PlayerMovement : MonoBehaviour, IOnEventCallback
{
    [SerializeField]
    Rigidbody myRB;
    [SerializeField]
    float speed;
    [SerializeField]
    Animator myAnimator;
    [SerializeField]
    PhotonView myPV;
    [SerializeField]
    AudioSource myAS;

    KeyCode upKey = KeyCode.W, downKey = KeyCode.S, rightKey = KeyCode.D, leftKey = KeyCode.A;

    bool hasControl = true, isRunning = false;

    float stepTimer = 0, stepSoundDelay = 0.5f;



    private void OnEnable()
    {
        //class can receive events
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte raiseEventCode = photonEvent.Code;

        
        if (raiseEventCode == RaiseEventCodes.LooseControl && myPV.IsMine)
        {
            //if looseControl event is raised, set boolean hasControl to false
            object[] data = (object[])photonEvent.CustomData;
            if ((Player)data[0] == myPV.Owner)
            {
                hasControl = false;
            }
        }

        if (raiseEventCode == RaiseEventCodes.GainControl && myPV.IsMine)
        {
            //if gainControl event is raised, set boolean hasControl to true
            object[] data = (object[])photonEvent.CustomData;
            if ((Player)data[0] == myPV.Owner)
            {
                hasControl = true;
            }
        }
    }



    void Update()
    {
        //only control your own player
        if (myPV.IsMine)
        {
            //only control player when active
            if (hasControl)
            {
                Vector3 directionForce;
                //when moving diagonally speed is altered so movement feels more linear
                if (Input.GetAxis("Horizontal") >= 0.5f && Input.GetAxis("Vertical") >= 0.5f ||
                    Input.GetAxis("Horizontal") <= -0.5f && Input.GetAxis("Vertical") <= -0.5f ||
                    Input.GetAxis("Horizontal") >= 0.5f && Input.GetAxis("Vertical") <= -0.5f ||
                    Input.GetAxis("Horizontal") <= -0.5f && Input.GetAxis("Vertical") >= 0.5f)
                {
                    directionForce = new Vector3(Input.GetAxis("Horizontal") * Time.deltaTime * speed * 0.8f, 0, Input.GetAxis("Vertical") * Time.deltaTime * speed * 0.8f);
                }
                else
                {
                    //if not moving diagonally use inputManager to move the player
                    directionForce = new Vector3(Input.GetAxis("Horizontal") * Time.deltaTime * speed, 0, Input.GetAxis("Vertical") * Time.deltaTime * speed);
                }
                //reset velocity each frame making movement feel more natural
                myRB.velocity = new Vector3(0,myRB.velocity.y,0);

                //We use addForce even though we reset velocity each frame so that collision detection works properly
                myRB.AddForce(directionForce);

                //check if player is moving
                if (directionForce.sqrMagnitude <= 0.5)
                {
                    isRunning = false;
                }
                else
                {
                    isRunning = true;
                }

                //play step sound after timer is over
                if (Time.time >= stepTimer)
                {
                    stepTimer = Time.time + stepSoundDelay;
                    if(isRunning)
                        myAS.Play();
                }
                //Animator receives speed so it can adjust animation
                myAnimator.SetFloat("Speed", directionForce.sqrMagnitude);
            }
        }
    }
}
