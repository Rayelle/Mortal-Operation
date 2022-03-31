using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using System.Linq;

public class MatchController : MonoBehaviour, IOnEventCallback, IInRoomCallbacks
{
    #region Variables

    [SerializeField]
    ScoreTracker myST;

    [SerializeField]
    TabScript myTabScript;

    [SerializeField]
    int minPlayerNumber;

    [SerializeField]
    List<Vector3> allSpawnPoints = new List<Vector3>();

    List<Player> connectedPlayers = new List<Player>();

    List<Player> playersAlive = new List<Player>();

    bool roundRunning = false, roundTimerActive = false, gameOverTimerActive = false,roundEndTimerActive=false;

    float roundTimer = 3, gameOverTimer = 0, gameOverDuration = 10, roundEndTimer = 0, roundEndDelay = 0.5f;

    private Player winningPlayer;

    #endregion


    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    void Update()
    {
        //Only the MatchController of the MasterClient is active
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            if (gameOverTimerActive)
            {
                if (Time.time >= gameOverTimer)
                {
                    //after a while gameOver-screen closes and everything starts anew
                    gameOverTimerActive = false;

                    object[] data = new object[] { };
                    RaiseEventOptions reo = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    PhotonNetwork.RaiseEvent(RaiseEventCodes.RestartRound, data, reo, SendOptions.SendReliable);

                    roundRunning = false;
                }
            }
            else
            {
                //if at any point players drop below threshhold current round is stopped
                if (PhotonNetwork.PlayerList.Count<Player>() < minPlayerNumber)
                {
                    roundRunning = false;

                }
                if (roundTimerActive)
                {
                    //if round timer is active: check for current timer and send events to update timer
                    roundTimer -= Time.deltaTime;
                    if (roundTimer <= 2)
                    {
                        UpdateRoundTimer("2");
                    }
                    if (roundTimer <= 1)
                    {
                        UpdateRoundTimer("1");
                    }
                    if (roundTimer <= 0)
                    {
                        //when timer reaches zero, send event to update timer and event for all players to regain control
                        UpdateRoundTimer("");

                        roundTimerActive = false;
                        roundTimer = 3;
                        foreach (Player p in PhotonNetwork.PlayerList)
                        {
                            object[] data = new object[] { p };
                            RaiseEventOptions reo = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                            PhotonNetwork.RaiseEvent(RaiseEventCodes.GainControl, data, reo, SendOptions.SendReliable);
                        }
                    }
                }
                if (!roundRunning && PhotonNetwork.PlayerList.Count<Player>() >= minPlayerNumber)
                {
                    //if enough players present, start first round
                    StartRound();

                    myST.ResetScore();

                }
                if (roundRunning && playersAlive.Count == 1 && !roundEndTimerActive)
                {
                    //if all but one player has been killed in a round
                    //start a short delay before restarting round and send score-info
                    roundEndTimer = Time.time + roundEndDelay;
                    roundEndTimerActive = true;
                    object[] data = new object[] { playersAlive[0] };
                    RaiseEventOptions reo = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    PhotonNetwork.RaiseEvent(RaiseEventCodes.RoundWin, data, reo, SendOptions.SendReliable);

                }
                if (roundEndTimerActive)
                {
                    if (Time.time >= roundEndTimer)
                    {
                        //after short round ending delay, reset the round
                        roundEndTimerActive = false;
                        StartRound();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Start a new round by resetting all player data and positions
    /// </summary>
    private void StartRound()
    {
        //check if a player one before starting a new round
        if (myST.GameOver())
        {
            //when match is over raise event which ends current match
            object[] data = new object[] { };
            RaiseEventOptions reo = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(RaiseEventCodes.RoundOver, data, reo, SendOptions.SendReliable);

            gameOverTimer = Time.time + gameOverDuration;
            gameOverTimerActive = true;

            return;
        }

        //remember all spawnpoints
        List<Vector3> freeSpawns = new List<Vector3>();

        foreach(Vector3 point in allSpawnPoints)
        {
            freeSpawns.Add(point);
        }
        //distribute players amongst all spawnpoints if there arent any free spawn points left, reuse one and add random deviation
        //players get spawnpoints through an event
        Vector3 currentSpawnPoint;
        foreach(Player currentPlayer in PhotonNetwork.PlayerList)
        {
            if (freeSpawns.Count == 0)
            {

                currentSpawnPoint = allSpawnPoints[Random.Range(0, allSpawnPoints.Count-1)] + new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
                SendPlayerRespawn(currentPlayer, currentSpawnPoint);
            }
            int i = Random.Range(0, freeSpawns.Count-1);
            currentSpawnPoint = freeSpawns[i];
            SendPlayerRespawn(currentPlayer, currentSpawnPoint);
            freeSpawns.Remove(freeSpawns[i]);
        }

        //remember all active players and round as running
        roundRunning = true;
        playersAlive = new List<Player>();

        foreach(Player p in PhotonNetwork.PlayerList)
        {
            playersAlive.Add(p);

            //take away control from all players until roundTimer is done
            object[] data = new object[] { p };
            RaiseEventOptions reo = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(RaiseEventCodes.LooseControl, data, reo, SendOptions.SendReliable);
        }

        //start the roundTimer
        roundTimerActive = true;
        UpdateRoundTimer("3");
    }

    /// <summary>
    /// Sends out an event to set the text of all roundstart-timers
    /// </summary>
    /// <param name="timerText"></param>
    private void UpdateRoundTimer(string timerText)
    {
        object[] data = new object[] { timerText };
        RaiseEventOptions reo = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(RaiseEventCodes.TimerChange, data, reo, SendOptions.SendReliable);
    }

    /// <summary>
    /// Send out an event to respawn a player at a certain point
    /// </summary>
    /// <param name="respawningPlayer"></param>
    /// <param name="spawnPoint"></param>
    private void SendPlayerRespawn(Player respawningPlayer, Vector3 spawnPoint)
    {
        object[] data = new object[] { respawningPlayer, spawnPoint };
        RaiseEventOptions reo = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(RaiseEventCodes.PlayerRespawn, data, reo, SendOptions.SendReliable);
    }


    public void OnEvent(EventData photonEvent)
    {
       
        byte raiseEventCode = photonEvent.Code;

        //register player-death in playersAlive list
        if (raiseEventCode == RaiseEventCodes.PlayerDeath)
        {
            object[] data = (object[])photonEvent.CustomData;
            playersAlive.Remove((Player)data[0]);
        }
    }

    #region IInRoomCallbacks
    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (roundRunning)
        {
            //if a player joins a running round, send him events to go to spectator mode and to loose control
            object[] data = new object[] { newPlayer };
            RaiseEventOptions reo = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(RaiseEventCodes.PlayerSpectate, data, reo, SendOptions.SendReliable);
            PhotonNetwork.RaiseEvent(RaiseEventCodes.LooseControl, data, reo, SendOptions.SendReliable);

            //add new player to scoreboard
            myST.AddPlayer(newPlayer);
        }

    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        //if players leave a running round, they need to be removed from playersAlive list
        if (playersAlive.Contains(otherPlayer))
        {
            playersAlive.Remove(otherPlayer);
        }
    }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        //implemented for interface, not used
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        //implemented for interface, not used
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
        //if host leaves during a match
        if (PhotonNetwork.PlayerList.Count<Player>() <= minPlayerNumber)
        {
            //if not enough players round is not active
            roundRunning = false;
        }
        else
        {
            //if still enough players restart round
            StartRound();
        }
    }
    #endregion
}
