using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class moves the camera to follow the player and serves as a spectator camera if the player is currently dead
public class FollowCamera : MonoBehaviour
{
    [SerializeField]
    Transform myPlayer;

    [SerializeField]
    Rigidbody myRB;

    [SerializeField]
    float followForce,speed;

    private bool camInitialized=false;

    Vector3 distanceToPlayer;

    bool spectatorModeOn=false;

    public bool SpectatorModeOn { set => spectatorModeOn = value; }
    public Vector3 DistanceToPlayer { get => distanceToPlayer; }

    /// <summary>
    /// Initialize Camera with player-reference and start movement
    /// </summary>
    /// <param name="player"></param>
    public void Init(GameObject player)
    {
        myPlayer = player.transform;
        distanceToPlayer = this.transform.position - myPlayer.position;
        camInitialized = true;
    }

    /// <summary>
    /// Stop cameramovement
    /// </summary>
    public void StopCamera()
    {
        camInitialized = false;
    }

    void Update()
    {
        if (camInitialized)
        {
            if (spectatorModeOn)
            {
                //control camera like you would control the player
                myRB.velocity = Vector3.zero;
                myRB.AddForce(new Vector3(Input.GetAxis("Horizontal") * Time.deltaTime * speed, 0, Input.GetAxis("Vertical") * Time.deltaTime * speed));
            }
            else
            {
                //follows player with slight delay to create feeling of motion
                myRB.velocity = Vector3.zero;
                myRB.AddForce(((myPlayer.position + distanceToPlayer) - this.transform.position) * Time.deltaTime * followForce);

            }
        }

    }
}
