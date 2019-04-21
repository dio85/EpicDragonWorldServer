﻿using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

/**
 * Author: Pantelis Andrianakis
 * Date: November 7th 2018
 */
class Config
{
    // --------------------------------------------------
    // Config File Definitions
    // --------------------------------------------------
    private static readonly string ACCOUNT_CONFIG_FILE = "Config" + Path.DirectorySeparatorChar + "Account.ini";
    private static readonly string LOGGING_CONFIG_FILE = "Config" + Path.DirectorySeparatorChar + "Logging.ini";
    private static readonly string PLAYER_CONFIG_FILE = "Config" + Path.DirectorySeparatorChar + "Player.ini";
    private static readonly string SERVER_CONFIG_FILE = "Config" + Path.DirectorySeparatorChar + "Server.ini";
    private static readonly string WORLD_CONFIG_FILE = "Config" + Path.DirectorySeparatorChar + "World.ini";

    // --------------------------------------------------
    // Account
    // --------------------------------------------------
    public static bool ACCOUNT_AUTO_CREATE;
    public static int ACCOUNT_MAX_CHARACTERS;

    // --------------------------------------------------
    // Logging
    // --------------------------------------------------
    public static bool LOG_CHAT;
    public static bool LOG_WORLD;

    // --------------------------------------------------
    // Player
    // --------------------------------------------------
    public static LocationHolder STARTING_LOCATION;
    public static List<int> VALID_SKIN_COLORS = new List<int>();

    // --------------------------------------------------
    // Server
    // --------------------------------------------------
    public static int SERVER_PORT;
    public static string DATABASE_CONNECTION_PARAMETERS;
    public static int DATABASE_MAX_CONNECTIONS;
    public static byte[] ENCRYPTION_SECRET_KEYWORD;
    public static byte[] ENCRYPTION_PRIVATE_PASSWORD;
    public static int MAXIMUM_ONLINE_USERS;
    public static double CLIENT_VERSION;

    // --------------------------------------------------
    // World
    // --------------------------------------------------
    public static float WORLD_MINIMUM_X;
    public static float WORLD_MAXIMUM_X;
    public static float WORLD_MINIMUM_Y;
    public static float WORLD_MAXIMUM_Y;
    public static float WORLD_MINIMUM_Z;
    public static float WORLD_MAXIMUM_Z;

    public static void Load()
    {
        ConfigReader accountConfigs = new ConfigReader(ACCOUNT_CONFIG_FILE);
        ACCOUNT_AUTO_CREATE = accountConfigs.GetBool("AccountAutoCreate", false);
        ACCOUNT_MAX_CHARACTERS = accountConfigs.GetInt("AccountMaxCharacters", 5);

        ConfigReader loggingConfigs = new ConfigReader(LOGGING_CONFIG_FILE);
        LOG_CHAT = loggingConfigs.GetBool("LogChat", true);
        LOG_WORLD = loggingConfigs.GetBool("LogWorld", true);

        ConfigReader playerConfigs = new ConfigReader(PLAYER_CONFIG_FILE);
        string[] startingLocation = playerConfigs.GetString("StartingLocation", "3924.109;67.42678;2329.238").Split(";");
        STARTING_LOCATION = new LocationHolder(float.Parse(startingLocation[0], CultureInfo.InvariantCulture), float.Parse(startingLocation[1], CultureInfo.InvariantCulture), float.Parse(startingLocation[2], CultureInfo.InvariantCulture), startingLocation.Length > 3 ? float.Parse(startingLocation[3], CultureInfo.InvariantCulture) : 0);
        foreach (string colorCode in playerConfigs.GetString("ValidSkinColorCodes", "F1D1BD;F1C4AD;E7B79C;E19F7E;AF7152;7E472E;4A2410;F7DDC0;F3D1A9;C5775A;B55B44;863923;672818;3F1508").Split(";"))
        {
            VALID_SKIN_COLORS.Add(Util.HexStringToInt(colorCode));
        }

        ConfigReader serverConfigs = new ConfigReader(SERVER_CONFIG_FILE);
        SERVER_PORT = serverConfigs.GetInt("ServerPort", 5055);
        DATABASE_CONNECTION_PARAMETERS = serverConfigs.GetString("DbConnectionParameters", "Server=127.0.0.1;User ID=root;Password=;Database=edws");
        DATABASE_MAX_CONNECTIONS = serverConfigs.GetInt("MaximumDbConnections", 50);
        ENCRYPTION_SECRET_KEYWORD = new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(serverConfigs.GetString("SecretKeyword", "SECRET_KEYWORD")));
        ENCRYPTION_PRIVATE_PASSWORD = Encoding.ASCII.GetBytes(serverConfigs.GetString("PrivatePassword", "1234567890123456"));
        MAXIMUM_ONLINE_USERS = serverConfigs.GetInt("MaximumOnlineUsers", 2000);
        CLIENT_VERSION = serverConfigs.GetDouble("ClientVersion", 1.0);

        ConfigReader worldConfigs = new ConfigReader(WORLD_CONFIG_FILE);
        WORLD_MINIMUM_X = worldConfigs.GetFloat("MinimumX", -558.8f);
        WORLD_MAXIMUM_X = worldConfigs.GetFloat("MaximumX", 4441.2f);
        WORLD_MINIMUM_Y = worldConfigs.GetFloat("MinimumY", -100f);
        WORLD_MAXIMUM_Y = worldConfigs.GetFloat("MaximumY", 1000f);
        WORLD_MINIMUM_Z = worldConfigs.GetFloat("MinimumZ", -1445.3f);
        WORLD_MAXIMUM_Z = worldConfigs.GetFloat("MaximumZ", 3554.7f);

        LogManager.Log("Configs: Initialized.");
    }
}