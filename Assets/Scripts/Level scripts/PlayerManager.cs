using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// PlayerManager script manages basic info about all players.


public class PlayerManager : MonoBehaviour
{
    // Unity editor vars, don't use in scripts or in real-time!
    public int INPUT_count;
    public List<string> INPUT_player_names;
    public List<int> INPUT_player_game_status;
    public List<int> INPUT_Team1;
    public List<int> INPUT_Team2;
    public Color INPUT_unknow_color;
    public List<Color> INPUT_colors;
    public Color INPUT_terminated_color;

    // static vars
    private static int count; // count of players
    private static List<string> player_names;
    private static List<int> player_game_status; // 1 - player playing, 0 - player defeated
    private static List<int> Team1; // list of players in Team 1
    private static List<int> Team2; // list of players in Team 2
    private static Color unknow_color; // color of devices which are identity-stealthed
    private static List<Color> colors; // colors of every player, player 0 is public player
    private static Color terminated_color; // color for terminated devices
    private static List<Resources> resources = new List<Resources>(); // holds informations about each player's resource counts

    private void Awake()
    {
        // use Unity Editor values in static variables
        count = INPUT_count;
        player_names = INPUT_player_names;
        player_game_status = INPUT_player_game_status;
        Team1 = INPUT_Team1;
        Team2 = INPUT_Team2;
        unknow_color = INPUT_unknow_color;
        colors = INPUT_colors;
        terminated_color = INPUT_terminated_color;

        // create resources list
        resources.Clear();
        for(int i = 0; i <= count; i++)
        {
            resources.Add(new Resources());
        }
    }
    public static int GetPlayerCount() // get number of players present in the scene
    {
        return count;
    }

    public static string GetPlayerName(int player) // get player's name
    {
        return player_names[player];
    }
    public static bool IsPlayerPlaying(int player)
    {
        if(player_game_status[player] == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static void DefeatPlayer(int player)
    {
        player_game_status[player] = 0;
    }
    public static int GetPlayerTeam(int player) // look if player is in a team 1, 2 or not a member of a team (returns 0)
    {
        if (Team1.Contains(player))
        {
            return 1;
        }
        if (Team2.Contains(player))
        {
            return 2;
        }
        return 0;
    }
    public static Color GetPlayerColor(int player) // get color of a given player, -1 is a stealthed device
    {
        if(player == -1)
        {
            return unknow_color;
        }
        else if (player == -2)
        {
            return terminated_color;
        }
        return colors[player];
    }
    public static bool ArePlayersFriendly(int player1, int player2)
    {

        if (player1 == player2) return true;
        if(GetPlayerTeam(player1) == 0 || GetPlayerTeam(player2) == 0)
        {
            return false;
        }
        else
        {
            return GetPlayerTeam(player1) == GetPlayerTeam(player2);
        }
    }

    private static int GetResource(int player, string resource_name)
    {
        return resources[player].Get(resource_name);
    }
    public static int GetPCS(int player)
    {
        return GetResource(player, Resources.PCS);
    }
    public static int GetCatCoin(int player)
    {
        return GetResource(player, Resources.CATCOIN);
    }
    public static int GetStolenData(int player)
    {
        return GetResource(player, Resources.STOLEN_DATA);
    }
    private static void AddResource(int player, int value, string resource_name)
    {
        resources[player].Add(value, resource_name);
    }
    public static void AddPCS(int player, int value)
    {
        AddResource(player, value, Resources.PCS);
    }
    public static void AddCatCoin(int player, int value)
    {
        AddResource(player, value, Resources.CATCOIN);
    }
    public static void AddStolenData(int player, int value)
    {
        AddResource(player, value, Resources.STOLEN_DATA);
    }
    private static bool RemoveResource(int player, int value, string resource_name)
    {
        return resources[player].Remove(value, resource_name);
    }
    public static bool RemovePCS(int player, int value)
    {
        return RemoveResource(player, value, Resources.PCS);
    }
    public static bool RemoveCatCoin(int player, int value)
    {
        return RemoveResource(player, value, Resources.CATCOIN);
    }
    public static bool RemoveStolenData(int player, int value)
    {
        return RemoveResource(player, value, Resources.STOLEN_DATA);
    }

}

class Resources
{
    private int pcs = 0; // pre-compiled scripts
    private int catcoin = 0; // CatCoin
    private int stolenData = 0; // Stolen data

    // const string keys
    public const string PCS = "pcs";
    public const string CATCOIN = "catcoin";
    public const string STOLEN_DATA = "stolendata";
    public int Get(string resource_name)
    {
        switch (resource_name)
        {
            case PCS:
                return pcs;
            case CATCOIN:
                return catcoin;
            case STOLEN_DATA:
                return stolenData;
            default:
                Debug.LogError("Unknown resource " + resource_name);
                break;
        }
        return 0;
    }
    public void Add(int value, string resource_name)
    {
        if (value < 0)
        {
            Debug.LogError("Can't add negative value");
            return;
        }

        switch (resource_name)
        {
            case PCS:
                pcs += value;
                break;
            case CATCOIN:
                catcoin += value;
                break;
            case STOLEN_DATA:
                stolenData += value;
                break;
            default:
                Debug.LogError("Unknown resource "+resource_name);
                break;
        }

    }
    public bool Remove(int value, string resource_name)
    {
        if (value < 0)
        {
            Debug.LogError("Use positive values");
            return false;
        }

        switch (resource_name)
        {
            case PCS:
                if (pcs >= value)
                {
                    pcs -= value;
                    return true;
                }
                else
                {
                    return false;
                }
            case CATCOIN:
                if (catcoin >= value)
                {
                    catcoin -= value;
                    return true;
                }
                else
                {
                    return false;
                }
            case STOLEN_DATA:
                if (stolenData >= value)
                {
                    stolenData -= value;
                    return true;
                }
                else
                {
                    return false;
                }
            default:
                Debug.LogError("Unknown resource " + resource_name);
                return true;
        }
    }
}