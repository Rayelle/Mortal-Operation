using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

/// <summary>
/// sets roundstart-timer to given value when event is raised
/// </summary>
public class RoundTimer : MonoBehaviour, IOnEventCallback
{

    [SerializeField]
    TextMeshProUGUI myTimerText;

    [SerializeField]
    TextMeshProUGUI myRoundText;

    private float myTimer;

    public void OnEvent(EventData photonEvent)
    {
        //set timer to given string when timer-event is raised
        byte raiseEventCode = photonEvent.Code;
        if (raiseEventCode == RaiseEventCodes.TimerChange)
        {
            object[] data = (object[])photonEvent.CustomData;
            myTimerText.text = (string)data[0];
            if((string)data[0] == "")
            {
                myTimer = 0;
            }
        }
    }

    public void Update()
    {
        myTimer += Time.deltaTime;
        myRoundText.text = ($"{Math.Round(myTimer, 1)}");
    }

    private void OnEnable()
    {
        //class can receive events
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
