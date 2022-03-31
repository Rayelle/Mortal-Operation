using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// this class is a container for all information asscosiated with scoreboard
/// </summary>
public class ScoreInfo
{
    Player pl;
    string username;
    int numberKills, numberDeaths, numberWins;

    public ScoreInfo(Player pl, string username, int numberKills, int numberDeaths, int numberWins)
    {
        this.pl = pl;
        this.username = username;
        this.numberKills = numberKills;
        this.numberDeaths = numberDeaths;
        this.numberWins = numberWins;
    }

    public string Username { get => username; set => username = value; }
    public int NumberKills { get => numberKills; set => numberKills = value; }
    public int NumberDeaths { get => numberDeaths; set => numberDeaths = value; }
    public int NumberWins { get => numberWins; set => numberWins = value; }
    public Player Pl { get => pl; set => pl = value; }
}
