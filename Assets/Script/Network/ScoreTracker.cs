using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

/// <summary>
/// Centralised score-tracker which is only active on the master client
/// </summary>
public class ScoreTracker : MonoBehaviour, IOnEventCallback
{
    List<ScoreInfo> allScore = new List<ScoreInfo>();

    int pointsToWin = 3;

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
        //only the master client tracks the score
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            byte raiseEventCode = photonEvent.Code;

            if (raiseEventCode == RaiseEventCodes.RoundWin)
            {
                //register RoundWin-information
                object[] data = (object[])photonEvent.CustomData;
                Player winningPlayer = (Player)data[0];
                foreach (ScoreInfo currentPlayerScore in allScore)
                {
                    if (currentPlayerScore.Pl == winningPlayer)
                    {
                        currentPlayerScore.NumberWins++;
                    }
                }
            }
            if (raiseEventCode == RaiseEventCodes.PlayerDeath)
            {
                //register kill-information
                object[] data = (object[])photonEvent.CustomData;
                Player killer = (Player)data[1];
                foreach (ScoreInfo currentPlayerScore in allScore)
                {
                    if (currentPlayerScore.Pl == killer)
                    {
                        currentPlayerScore.NumberKills++;
                    }
                }

                //register death-information
                Player deadPlayer = (Player)data[0];
                foreach (ScoreInfo currentPlayerScore in allScore)
                {
                    if (currentPlayerScore.Pl == deadPlayer)
                    {
                        currentPlayerScore.NumberDeaths++;
                    }
                }
            }
            //when clients ask for a score-update raise event to send all clients current score data
            if(raiseEventCode == RaiseEventCodes.RequestScore)
            {
                TransmitScore();
            }

        }
    }

    private void TransmitScore()
    {
        foreach (ScoreInfo si in allScore)
        {
            //remove disconnected players from list
            if (!PhotonNetwork.PlayerList.Contains(si.Pl))
            {
                //send event to all clients to remove disconnected clients from score-list
                object[] data = new object[] { si.Pl };
                RaiseEventOptions reo = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(RaiseEventCodes.RemoveScoreInfo, data, reo, SendOptions.SendReliable);
                allScore.Remove(si);
            }
            else
            {
                //send each entry in score-list to clients
                object[] data = new object[] { si.Pl, si.Username, si.NumberKills,si.NumberDeaths, si.NumberWins  };
                RaiseEventOptions reo = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(RaiseEventCodes.TransmitScore, data, reo, SendOptions.SendReliable);
            }
        }
    }

    public void ResetScore()
    {
        //resets the score to be empty
        allScore = new List<ScoreInfo>();

        //raises event so each client also empties score-list
        object[] data = new object[] { };
        RaiseEventOptions reo = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(RaiseEventCodes.ResetAllScore, data, reo, SendOptions.SendReliable);

        //newly adds all currently connected players to score-list without any kills/deaths/wins
        foreach (Player pl in PhotonNetwork.PlayerList)
        {
            allScore.Add(new ScoreInfo(pl, pl.NickName, 0, 0, 0));
        }
    }

    /// <summary>
    /// Add a player to the list. This is called when a player connects to a running game.
    /// </summary>
    /// <param name="newPlayer"></param>
    public void AddPlayer(Player newPlayer)
    {
        allScore.Add(new ScoreInfo(newPlayer, newPlayer.NickName, 0, 0, 0));
    }

    /// <summary>
    /// Returns true if a player won the match
    /// </summary>
    /// <returns></returns>
    public bool GameOver()
    {
        foreach(ScoreInfo si in allScore)
        {
            if (si.NumberWins >= pointsToWin)
            {
                return true;
            }
        }
        return false;
    }

}
