using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabScript : MonoBehaviour, IOnEventCallback
{
    [SerializeField] GameObject TabMenu,WinUI;
    [SerializeField]
    PersonalScoreTracker myPST;
    [SerializeField]
    float updateInterval;


    float updateTime = 0;

    bool updated = false, endScreenActive=false;

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

        //when match is over endscreen is permanently displayed
        if (raiseEventCode == RaiseEventCodes.RoundOver)
        {
            endScreenActive = true;
            WinUI.SetActive(true);
        }
        //resets tabmenu when next match starts
        if(raiseEventCode == RaiseEventCodes.RestartRound)
        {
            endScreenActive = false;
            WinUI.SetActive(false);
        }
    }

    void Update()
    {
        if (endScreenActive)
        {
            //when the end-screen is active Score should always be displayed
            TabMenu.gameObject.SetActive(true);

            if (Time.time >= updateTime)
            {
                //Score is only requested and updated in certain intervals to safe network traffic
                updateTime = Time.time + updateInterval;
                myPST.RequestScoreInfo();
                myPST.UpdatePlayerStats();
            }
        }
        else
        {
            //opens tab-menu while Tab-key is held down
            if (Input.GetKey(KeyCode.Tab))
            {
                if (Time.time >= updateTime)
                {
                    //Score is only requested and updated in certain intervals to safe network traffic
                    updateTime = Time.time + updateInterval;
                    myPST.RequestScoreInfo();
                    myPST.UpdatePlayerStats();
                }
                TabMenu.gameObject.SetActive(true);
            }
            else
            {
                //while tab is not pressed, tabmenu is inactive
                TabMenu.gameObject.SetActive(false);
            }

        }

        
    }
}
