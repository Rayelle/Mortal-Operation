using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;


/// <summary>
/// class is responsible for networking inside mainMenu
/// </summary>
public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Button StartButton;
    [SerializeField]
    byte MaxPlayers;
    [SerializeField]
    GameObject matchControllerPrefab;

    [SerializeField]
    TMP_InputField usernameInput;




    void Start()
    {
        //set up photonNetwork
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }


    void Update()
    {
        //start button can only be presed when photonNetwork is ready
        StartButton.interactable = PhotonNetwork.IsConnectedAndReady;

    }

   

    public void ConnectButton()
    {
        //username is set according to inputfield
        if (usernameInput.text != "")
        {
            PhotonNetwork.LocalPlayer.NickName = usernameInput.text;
        }
        else
        {
            PhotonNetwork.LocalPlayer.NickName = $"Player {Random.Range(1000,9999)}";
        }
        //try to join an existing room
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //if no existing room is found, create a new one
        CreateRoom();
    }


    private void CreateRoom()
    {
        //room is created with random name and maxPlayer limitation
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = MaxPlayers;
        PhotonNetwork.CreateRoom("Room " + Random.Range(10000, 99999), ro);
    }

    public override void OnCreatedRoom()
    {
        //load basicMap when room is created
        PhotonNetwork.LoadLevel(1);        
    }

    public override void OnJoinedRoom()
    {
        
    }
}
