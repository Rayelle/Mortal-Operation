using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// local score tracker which updates score-information in each client
/// </summary>
public class PersonalScoreTracker : MonoBehaviour, IOnEventCallback
{
    List<ScoreInfo> allScore = new List<ScoreInfo>();

    [SerializeField]
    List<TextMeshProUGUI> playerNames = new List<TextMeshProUGUI>();
    [SerializeField]
    List<TextMeshProUGUI> playerKD = new List<TextMeshProUGUI>();
    [SerializeField]
    List<TextMeshProUGUI> playerWins = new List<TextMeshProUGUI>();

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

        
        if(raiseEventCode == RaiseEventCodes.ResetAllScore)
        {
            //reset score if round is restarted
            allScore = new List<ScoreInfo>();
        }

        if(raiseEventCode == RaiseEventCodes.RemoveScoreInfo)
        {
            //remove score of a disconnecting player from the list
            object[] data = (object[])photonEvent.CustomData;
            Player toRemove = (Player)data[0];
            foreach (ScoreInfo si in allScore)
            {
                if(si.Pl== toRemove)
                {
                    allScore.Remove(si);
                }
            }
        }

        if (raiseEventCode == RaiseEventCodes.TransmitScore)
        {
            //receive score-info on a single player
            object[] data = (object[])photonEvent.CustomData;
            bool isContained = false;
            foreach(ScoreInfo si in allScore)
            {
                //if player is already in the score-list, update his information
                if (si.Pl == data[0])
                {
                    si.Username = (string)data[1];
                    si.NumberKills = (int)data[2];
                    si.NumberDeaths = (int)data[3];
                    si.NumberWins = (int)data[4];
                    isContained = true;
                }
            }
            if (!isContained)
            {
                //if player is not in the score-list add a new entry including all score-information
                allScore.Add(new ScoreInfo((Player)data[0], (string)data[1], (int)data[2], (int)data[3], (int)data[4]));
            }
        }
    }

    /// <summary>
    /// updates playerstats inside canvas
    /// </summary>
    public void UpdatePlayerStats()
    {
        //first sort score-information by number of kills
        SortScoreInfo(allScore);
        int index = 0;
        foreach (ScoreInfo si in allScore)
        {
            //only displays top 5 players
            if (index >= 5)
                break;
            //set score-info into each textfield inside the canvas
            playerNames[index].text = si.Username;
            playerKD[index].text = $"{si.NumberKills}/{si.NumberDeaths}";
            playerWins[index].text = $"{si.NumberWins}";

            index++;
        }
    }

    /// <summary>
    /// raises event which asks the ScoreTracker for all score-info
    /// </summary>
    public void RequestScoreInfo()
    {
        object[] data = new object[] { };
        RaiseEventOptions reo = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent(RaiseEventCodes.RequestScore, data, reo, SendOptions.SendReliable);
    }

    /// <summary>
    /// bubble-sort score based on number of kills
    /// </summary>
    /// <param name="siList"></param>
    private void SortScoreInfo(List<ScoreInfo> siList)
    {
        //list with 0 or 1 element is already sorted
        if (siList.Count <= 1)
        {
            return;
        }

        //sort list based on neighbouring elements
        bool sorted=false;
        while (!sorted)
        {
            sorted = true;
            ScoreInfo lastElement = siList[0];
            for (int i = 1; i < siList.Count; i++)
            {
                if (siList[i].NumberKills > lastElement.NumberKills)
                {
                    ScoreInfo tmp = siList[i];
                    siList[i] = lastElement;
                    siList[i - 1] = tmp;
                    sorted = false;
                }
            }
        }
    }

    //public string WinningPlayer()
    //{
    //    int mostWins = 0;
    //    string winningPlayer = "winningPlayerNotFound";
    //    foreach(ScoreInfo si in allScore)
    //    {
    //        if (si.NumberWins > mostWins)
    //        {
    //            mostWins = si.NumberWins;
    //            winningPlayer = si.Username;
    //        }
    //    }
    //    return winningPlayer;
    //}
}
