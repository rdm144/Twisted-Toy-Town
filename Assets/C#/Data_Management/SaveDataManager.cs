using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic; //lists
using System.IO; //file management
using System;
using JetBrains.Annotations;

public enum SaveType { Config, Player }

public class SaveDataManager
{
    public static ConfigData configdata;
    public static PlayerData playerdata;
    public static string savePath { get; private set; } = Application.persistentDataPath + "/Data";

    public static void Save(SaveType type)
    {
        BinaryFormatter serializer = new BinaryFormatter();
        FileStream stream;

        switch (type)
        {
            case SaveType.Config: // Save configuration data
                stream = new FileStream(savePath + "/Config.snzx", FileMode.Create);
                serializer.Serialize(stream, configdata);
                break;
            case SaveType.Player: // Save player progress
                stream = new FileStream(savePath + "/Save.snzx", FileMode.Create);
                serializer.Serialize(stream, playerdata);
                break;
            default:
                Debug.LogError("Cannot save a save type of " + type + ". Saving PlayerData instead.");
                stream = new FileStream(savePath + "/Save.snzx", FileMode.Create);
                serializer.Serialize(stream, playerdata);
                break;
        }
        stream.Close();
    }

    public static void Load(SaveType type)
    {
        BinaryFormatter serializer = new BinaryFormatter();
        FileStream stream;

        switch (type)
        {
            case SaveType.Config: // Load configuration data
                if (File.Exists(savePath + "/Config.snzx"))
                {
                    stream = new FileStream(savePath + "/Config.snzx", FileMode.Open);
                    configdata = (ConfigData)serializer.Deserialize(stream);
                    stream.Close();
                }
                else
                    RebuildFile(type);
                break;
            case SaveType.Player: // Load player data
                if (File.Exists(savePath + "/Save.snzx"))
                {
                    stream = new FileStream(savePath + "/Save.snzx", FileMode.Open);
                    playerdata = (PlayerData)serializer.Deserialize(stream);
                    stream.Close();
                }
                else
                    RebuildFile(type);
                break;
            default:
                Debug.LogError("Cannot load a save type of " + type + ". Loading PlayerData instead.");
                if (File.Exists(savePath + "/Save.snzx"))
                {
                    stream = new FileStream(savePath + "/Save.snzx", FileMode.Open);
                    playerdata = (PlayerData)serializer.Deserialize(stream);
                    stream.Close();
                }
                else
                    RebuildFile(type);
                break;
        }
    }

    public static void RebuildFile(SaveType type)
    {
        Directory.CreateDirectory(savePath);
        Debug.Log("New save of type " + type + " is located at: " + savePath);

        // This will rebuild any file specified by fileName.extension
        switch (type)
        {
            case SaveType.Config:
                Debug.Log("Rebuilding config.snzx file");
                configdata = new ConfigData();
                
                Keybind u = new Keybind();
                u.name = "Up";
                u.key = KeyCode.W;
                configdata.keybindList.Insert(0, u);
                Keybind d = new Keybind();
                d.name = "Down";
                d.key = KeyCode.S;
                configdata.keybindList.Insert(1, d);
                Keybind l = new Keybind();
                l.name = "Left";
                l.key = KeyCode.A;
                configdata.keybindList.Insert(2, l);
                Keybind r = new Keybind();
                r.name = "Right";
                r.key = KeyCode.D;
                configdata.keybindList.Insert(3, r);
                Keybind c = new Keybind();
                c.name = "Confirm";
                c.key = KeyCode.Space;
                configdata.keybindList.Insert(4, c);
                Keybind b = new Keybind();
                b.name = "Back";
                b.key = KeyCode.Escape;
                configdata.keybindList.Insert(5, b);

                configdata.screenSettings.width = Screen.width;
                configdata.screenSettings.height = Screen.height;
                configdata.screenSettings.isFullscreen = Screen.fullScreen;
                configdata.screenSettings.vsync = 1;
                configdata.screenSettings.FPScounter = false;

                Save(SaveType.Config);
                break;

            case SaveType.Player:
                Debug.Log("Rebuilding save.snzx file");
                playerdata = new PlayerData();
                playerdata.progress.playTimeSeconds = 0;
                playerdata.progress.playTime = TimeSpan.Zero;
                playerdata.progress.startTime = DateTime.Now;
                playerdata.progress.SceneNumber = 1;
                playerdata.progress.respawnPointX = 0;
                playerdata.progress.respawnPointZ = 0;
                playerdata.party.Insert(0, new PartyMember("Jimmy", 0, 10, 10));
                playerdata.party.Insert(1, new PartyMember("Meddy", 1, 10, 10));

                Save(SaveType.Player);
                break;

            default:
                //Debug.Log("SaveDataManager doesn't know which file needs repairs.");
                break;
        }
    }
}

[Serializable]
public class ConfigData
{
    public AudioSettings audioSettings = new AudioSettings();
    public ScreenSettings screenSettings = new ScreenSettings();

    public List<Keybind> keybindList = new List<Keybind>();

    public List<Keybind> getKeybinds()
    {
        return keybindList;
    }
}

[Serializable]
public class AudioSettings
{
    readonly float DEFAULT_MASTER = 1;
    readonly float DEFAULT_MUSIC = 0.4f;
    readonly float DEFAULT_SFX = 0.75f;

    public float Master { get; private set; }
    public float Music { get; private set; }
    public float SFX { get; private set; }

    // Cannot use  Mathf.Clamp01(value) with get; and set;, because it SOMEHOW leads to StackOverflowException.
    public void SetMaster(float f) { Master = Mathf.Clamp01(f); }
    public void SetMusic(float f) { Music = Mathf.Clamp01(f); }
    public void SetSFX(float f) { SFX = Mathf.Clamp01(f); }

    public AudioSettings()
    {
        SetMaster(DEFAULT_MASTER);
        SetMusic(DEFAULT_MUSIC);
        SetSFX(DEFAULT_SFX);
    }
}

[Serializable]
public class ScreenSettings
{
    public int width;
    public int height;
    public int antiAliasing;
    public int refreshRate { get; private set; }
    public int vsync;
    public bool isFullscreen;
    public bool FPScounter;
    public float bloom;
    public float vignette;
    public float contrast;
    public float chromaticAberration;

    public ScreenSettings()
    {
        width = Screen.width;
        height = Screen.height;
        antiAliasing = 3;
        refreshRate = (int)Screen.currentResolution.refreshRateRatio.value;
        vsync = 1;
        isFullscreen = Screen.fullScreen;
        FPScounter = false;
        bloom = 2;
        vignette = 1;
        contrast = 2;
        chromaticAberration = 1;
    }
}

[System.Serializable]
public class Keybind
{
    public string name;
    public KeyCode key;
}

[System.Serializable]
public class PlayerData
{
    public Progress progress = new Progress();
    public List<PartyMember> party = new List<PartyMember>();
    // public List<Item> inventory = new List<Item>();
}

[System.Serializable]
public class PartyMember
{
    public string name;
    public int partyIndex;
    public int health { get { return currentHealth; } set { if (value <= 0) currentHealth = 1; else currentHealth = value; }}
    private int currentHealth;
    public int maxHealth;

    public PartyMember()
    {
        name = "Mr.Meaty";
        partyIndex = 0;
        health = 5;
        maxHealth = 10;
    }

    public PartyMember(string newName, int newIndex, int currentHP, int maxHP)
    {
        name = newName;
        partyIndex = newIndex;
        health = currentHP;
        maxHealth = maxHP;
    }

    public void SetValues(string newName, int newIndex, int currentHP, int maxHP)
    {
        name = newName;
        partyIndex = newIndex;
        health = currentHP;
        maxHealth = maxHP;
    }
}

[Serializable]
public class MetaStats
{
    public int TotalPlayTimeSeconds;
    public int GameOvers;
    public DateTime startTime; // used to keep track of current play session time
    public TimeSpan playTime; // used to keep track of current play session time
    public int Encounters;

    public MetaStats()
    {
        TotalPlayTimeSeconds = 0;
        GameOvers = 0;
        playTime = TimeSpan.Zero;
        startTime = DateTime.Now;
        Encounters = 0;
    }
}

[Serializable]
public class Progress
{
    public int SceneNumber;
    public float respawnPointX;
    public float respawnPointZ;
    public int playTimeSeconds;
    public DateTime startTime;
    public TimeSpan playTime;
    public int CurrentEncounterID;
    public List<EnemyList> enemyLists;

    public Progress()
    {
        SceneNumber = 0;
        respawnPointX = 0;
        respawnPointZ = 0;
        enemyLists = new List<EnemyList>();
    }
}

[System.Serializable]
public class EnemyList
{
    public int sceneNumber;
    public List<int> graveyard;
    public List<EnemyPosition> enemyPositions;

    public EnemyList()
    {
        sceneNumber = 0;
        graveyard = new List<int>();
        enemyPositions = new List<EnemyPosition>();
    }

    public EnemyList(int number)
    {
        sceneNumber = number;
        graveyard = new List<int>();
        enemyPositions = new List<EnemyPosition>();
    }
}

[System.Serializable]
public class EnemyPosition
{
    public float x;
    public float y;
    public float z;

    public EnemyPosition()
    {
        x = y = z = 0;
    }
}