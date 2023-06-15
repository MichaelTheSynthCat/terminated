using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperClass : MonoBehaviour
{
    public static string PrintSlavePenalty(OperatorScript target_operator)
    {
        return (target_operator.GetSlavePenalty(false) * 100.0f).ToString("0") + "%";
    }
}

public class CommandsDefinitions
{
    // This class holds various string definitions.

    // operator related commands
    public const string idle = "idle";
    public const string scan = "scan";
    public const string tcpcatch = "tcpcatch";
    public const string antivirus_scan = "antivirusscan";
    public const string cyberattack = "cyberattack";
    public const string trojan = "trojanprocess";
    public const string break_firewall = "breakfirewall";
    public const string inject_spyworm = "injectspyworm";
    public const string delete_log = "deletelog";
    public const string counterattack = "counterattack";
    public const string create_stasis = "createstasis";
    public const string repair = "repair";
    public const string create_vpn = "createvpn";
    public const string create_slave = "createslave";
    public const string install_software = "installsoftware";
    public const string update_bios = "update_bios";
    public const string download_data = "downloaddata";
    public const string terminate = "terminate";
    public const string uninstall = "uninstall";

    // operator PCS costs
    public const string extra_slave = "extra_slave";
    public const string extra_update_bios = "extra_update_bios";

    // computer softwares
    public const string vpn_connection = "vpn_connection";
    public const string slave = "slave";
    public const string generate_pcs = "generate_pcs";
    public const string mine_catcoin = "minecatcoin";
    public const string autoantivirus = "autoantivirus";
    public const string autorepair = "autorepair";
    public const string botnet = "botnet";

    public const string botnet_attack = "botnet_attack";

    public static Dictionary<string, string> formatted_installs = new Dictionary<string, string>()
    {
        {generate_pcs, "PCS-gen" },
        {mine_catcoin, "CC-miner" },
        {autoantivirus, "Antivirus" },
        {autorepair, "Autorepair" },
        {botnet, "Botnet" },
    };

    public static string GetInstallName(string install)
    {
        return formatted_installs[install];
    }
}
public class CommandCodes
{
    // This class holds various string definitions.

    // operator related commands
    public const int idle = 0;
    public const int scan = 1;
    public const int tcpcatch = 2;
    public const int antivirus_scan = 3;
    public const int cyberattack = 4;
    public const int trojan = 5;
    public const int break_firewall = 6;
    public const int inject_spyworm = 7;
    public const int delete_log = 8;
    public const int counterattack = 9;
    public const int create_stasis = 10;
    public const int repair = 11;
    public const int create_vpn = 12;
    public const int create_slave = 13;
    public const int install_software = 14;
    public const int update_bios = 15;
    public const int download_data = 16;
    public const int terminate = 17;
    public const int uninstall = 18;

    // operator PCS costs
    public const int extra_slave = 81;
    public const int extra_update_bios = 82;

    // computer softwares
    public const int vpn_connection = 51;
    public const int slave = 52;
    public const int generate_pcs = 53;
    public const int mine_catcoin = 54;
    public const int autoantivirus = 55;
    public const int autorepair = 56;
    public const int botnet = 57;

    public const int botnet_attack = 70;
}
public class NameGenerator
{
    public const string TOKEN = "@";
    public const char CHAR_U_TOKEN = 'X';
    public const char NUM_TOKEN = '0';
    public const string ALPHABET = "ABCDEFGHIJKLMNOPQRSTVUWXYZ";

    private static List<string> code_templates = new List<string>()
    {
        "X", "0", "X00", "X-0", "XXX", "X0",
    };
    private static List<string> model_names = new List<string>()
    {
        "Classic", "NextGen", "Compact", "Lite", "Idea", "Legionaire", "Inspire", "Vision", "Zen", "Elite", "Plus", "Ultra",
        "One", "Destroyer", "Dominator", "Genius", "Silenzio", "Olgesaf",
    };
    private static List<string> computer_brands = new List<string>()
    {
        "Azuz", "Kenovo", "Shiba-To", "ph", "Racer", "Komputer", "Mapple", "Dellete", "Platinum", "MSY",
        "Sasus", "Deli", "Ni", "Ziaomi", "Erock", "Pochitach", "iDigi", "Olgesaf",
    };
    private static List<string> router_brands = new List<string>()
    {
        "ZenLink", "Azuz", "SysLink", "Xyzel", "Mapple", "A-Link", "KSI", "iRouter", "ConnectThat", "Makrotik", "Chuiawei",
        "Cenda", "Ubiduti",
    };
    private static List<string> phone_brands = new List<string>()
    {
        "Ziaomi", "Ni", "Zambzung", "Kenovo", "Sasus", "Chuiawei", "Dishonor", "Mapple", "Oneminus", "Yeskia", "Finokia",
        "Macrosoft", "youPhone", "Giigle", "Zoni", "GL", "BlueBerry", "Eric",
    };
    private static List<string> device_brands = new List<string>()
    {
        "Philip", "Ranasonic", "Oraawaa", "Chitahi", "Mapple", "Biseus", "A-link", "Elbrock", "CySUni", "Evovo", "Moneywell",
        "Mikvision", "GL", "Zambzung", "Panasic", "Fylyps", "Life's not good", "Unlogictech", "Pepson", "ph", "Azuz", "Ziaomi",
        "PT-Link", "Giggle", "Macrosoft", "iksBox", "Soulstation", "No-tendo",
    };

    private static List<string> usernames = new List<string>()
    {
        "John", "Marosh", "Adam", "Michael", "User", "Jeremy", "Bobby", "Gordon", "Riciardo", "Li", "Riciardo",
        "Rytlock", "Mark", "Roman", "Stanley", "Jimmy", "Misko", "Daniell", "Estelle", "Fero", "Juan", "Big Chungus",
        "Hazune miku", "Master Chef", "Funny dank", "pyro", "SLAYER", "princess", "Mario", "Probius", "Syntherceptor",
        "Keanu", "Wick", "Elon", "Quack", "engie_man", "PRO GAMER", "<nameROG>", "username", "penis-man", "Adolf", "Karen",
        "BeTtYdElPhInNe", "NameMeCarlson", "Pepe the frog", "kahbfjkasb", "qeuqwoiugnxmx", "hfsdhj", "guest", "incognito",
        "dont hack pls", "free robux", "kpop",
    };
    public static string GenerateName(string[] arguments)
    {
        string name = "";
        foreach(string arg in arguments)
        {
            if (arg == "") continue;
            if(arg[0] == '!')
            {
                name += arg.Substring(1) + " ";
                continue;
            }
            switch (arg)
            {
                case "code":
                    string template = code_templates[Random.Range(0, code_templates.Count)];
                    string code = "";
                    foreach(char character in template)
                    {
                        if(character == CHAR_U_TOKEN)
                        {
                            code += ALPHABET[Random.Range(0, ALPHABET.Length)];
                        }
                        else if (character == NUM_TOKEN)
                        {
                            code += Random.Range(0, 10).ToString();
                        }
                        else
                        {
                            code += character;
                        }
                    }
                    name += code + " ";
                    break;
                case "model":
                    name += model_names[Random.Range(0, model_names.Count)] + " ";
                    break;
                case "computer":
                    name += computer_brands[Random.Range(0, computer_brands.Count)] + " ";
                    break;
                case "router":
                    name += router_brands[Random.Range(0, router_brands.Count)] + " ";
                    break;
                case "phone":
                    name += phone_brands[Random.Range(0, phone_brands.Count)] + " ";
                    break;
                case "device":
                    name += device_brands[Random.Range(0, device_brands.Count)] + " ";
                    break;
                case "user":
                    name += usernames[Random.Range(0, usernames.Count)] + " ";
                    break;
                default:
                    Debug.LogError("NameGenerator: Invalid argument - " + arg);
                    break;
            }
        }
        return name.Substring(0, name.Length-1);
    }
}