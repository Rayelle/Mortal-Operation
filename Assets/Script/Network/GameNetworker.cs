using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using UnityEngine.UI;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using TMPro;


/// <summary>
/// Class is responsible for setting up player and camera inside a level
/// </summary>
public class GameNetworker : MonoBehaviour
{
    [SerializeField]
    GameObject camPrefab;

    [SerializeField]
    List<Transform> spawnpoints = new List<Transform>();

    [SerializeField]
    Vector3 camOffset, camRotation;

    [SerializeField]
    Slider playerHealthBar;

    [SerializeField]
    private Image sniperI, smgI;

    [SerializeField]
    private TextMeshProUGUI ammoCounter;



    void Start()
    {
        int i = Random.Range(0, spawnpoints.Count);
        //instantiates a player in all clients at random position from spawnpoints-list
        GameObject myPlayer = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawnpoints[i].position, Quaternion.identity);

        //sets reference for healthbar
        PlayerData newPlayerData = myPlayer.GetComponent<PlayerData>();
        newPlayerData.MyHealthBar = playerHealthBar;

        
        Quaternion camQ = new Quaternion();
        camQ.eulerAngles = camRotation;

        //instatiates a camera locally using given rotation and offset from player
        GameObject myCam = Instantiate(camPrefab, myPlayer.transform.position + camOffset, camQ);
        FollowCamera newFollowCam = myCam.GetComponent<FollowCamera>();

        //sets reference for player and camera
        newFollowCam.Init(myPlayer);
        newPlayerData.MyCamera = newFollowCam;

        //sets reference from WeaponController to the local GUI
        WeaponController myWeaponController = myPlayer.GetComponentInChildren<WeaponController>();
        myWeaponController.SniperI = sniperI;
        myWeaponController.SmgI = smgI;
        myWeaponController.AmmoCounter = ammoCounter;
    }  
    
}
