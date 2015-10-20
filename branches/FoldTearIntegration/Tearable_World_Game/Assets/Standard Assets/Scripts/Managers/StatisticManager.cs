/// <summary>
/// 
/// FILE: Statistic manager
/// 
/// DESCRIPTION:
/// 	This file is used to keep track of the player's statistics in game for achievements,
/// 	saving, and loading game sessions
/// 
/// AUTHORS: Tearable World Team (Crumpled Up Games' Engineers)
/// 			-> John Crocker, Justin Telmo, ... (ADD YOUR NAME HERE!!!!)
/// 
/// </summary>
/// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class VariablesForSaving
{

    /// <summary>
    /// This dictionary is used to keep track of the highest score for each
    /// level the player has achieved
    /// </summary>
    public Dictionary<int, int> LevelScoresDictToSave;

    public VariablesForSaving()
    {
        LevelScoresDictToSave = new Dictionary<int, int>();
    }

    public VariablesForSaving(SerializationInfo info, StreamingContext context)
    {
        foreach (int key in LevelScoresDictToSave.Keys)
        {
            LevelScoresDictToSave[key] = (int)info.GetValue("LevelScoresDictToSave[key]", typeof(int));
        }
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        foreach (int key in LevelScoresDictToSave.Keys)
        {
            info.AddValue("LevelScoresDictToSave[key]", typeof(int));
        }
    }
}


[System.Serializable]
public class StatisticManager : MonoBehaviour
{

    #region Fields

    /// <summary>
    /// This represents the player's saved game, where the needed data is stored for
    /// launching a reloaded game session
    /// </summary>
    //private SavedData savedGame;

    /// <summary>
    /// The number levels completed represents the current number
    /// of levels the player has compeleted
    /// </summary>
    private int numLevelsCompleted;

    /// <summary>
    /// This is used to flag whether or not a player is currently trying to 
    /// resume a saved game session
    /// </summary>
    private bool resumeSession;

    /// <summary>
    /// This represents the total number of levels Tearable world has to offer
    /// the player
    /// </summary>
    private const int TOTAL_NUM_LEVELS = 1;

    /// <summary>
    /// This represents current high score for the only level above
    /// TODO: This will change once the player has gained points in game,
    /// updating the levelScores dictionary accordinly
    /// </summary>
    private const int LEVEL_HIGH_SCORE = 1;


    /// <summary>
    /// The name of the file storing data to be saved/loaded
    /// </summary>
    private const string FILE_NAME = "savedData";

    /// <summary>
    /// This dictionary is used to keep track of the highest score for each
    /// level the player has achieved
    /// </summary>
    public Dictionary<int, int> LevelScoresDict;

    #endregion

    #region Methods

    public string GetMobileDocumentsPath()
    {
        // Your game has read+write access to /var/mobile/Applications/XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX/Documents 
        // Application.dataPath returns              
        // /var/mobile/Applications/XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX/myappname.app/Data 
        // Strip "/Data" from path 
        string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
        // Strip application name 
        path = path.Substring(0, path.LastIndexOf('/'));
        return path + "/Documents";
    }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    private void Start()
    {

    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {

    }

    /// <summary>
    /// Saves the game and all stats needed for next relaunch, TODO: IMPLEMENT!
    /// </summary>
    /// <param name='currentLevel'>
    /// Current level represents the current level being saved
    /// </param>
    /// <param name='score'>
    /// Score represents the score of the player to be saved
    /// </param>
    public void SaveGame(int currentLevel, int score)
    {
        VariablesForSaving variablesToSave = new VariablesForSaving();

        variablesToSave.LevelScoresDictToSave[currentLevel] = score;

        WriteDictToFile(FILE_NAME, variablesToSave);

        Debug.Log("Info Saved");
    }

    /// <summary>
    /// Loads the save game for the player
    /// </summary>
    public void LoadSaveGame()
    {
        VariablesForSaving variablesToSave = new VariablesForSaving();

        ReadDictFromFile(FILE_NAME, variablesToSave);

        Debug.Log("Info Loaded");

        Debug.Log("Level One Score " + variablesToSave.LevelScoresDictToSave[1].ToString());

        LevelScoresDict = variablesToSave.LevelScoresDictToSave;

    }

    /// <summary>
    /// Returns a string with the path to the specified filename
    /// given as the input parameter.  The save path differs depending
    /// on which platform the user is using.
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public string PathForDocumentFile(string filename)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(Path.Combine(path, "Documents"), filename);
        }

        else if (Application.platform == RuntimePlatform.Android)
        {
            string path = Application.persistentDataPath;
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(path, filename);
        }

        else
        {
            string path = Application.dataPath;
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(path, filename);
        }
    }

    /// <summary>
    /// Iterates through the saved file specified from the input paramater
    /// one line at a time.  The saved document should have as many lines
    /// as there are dictionary entries and assigns a dictionary key to a 
    /// corresponding line in the file e.g. (LevelScoreDict[Level_1] = first line read from the file
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public string ReadDictFromFile(string filename, VariablesForSaving variablesToSave)
    {

#if !WEB_BUILD

        // path to the specified file name
        string path = PathForDocumentFile(filename);

        // check to see if the path exists, otherwise return null
        if (File.Exists(path))
        {
            // using file stream to actually find and open/create the file which we are
            // reading from.
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);

            // stream writer allows us to actually read
            // the file
            StreamReader sr = new StreamReader(file);

            // temporary string to read from
            string str = null;

            // dictionary starts at entry 1, corresponding to level number
            int key = 1;

            // read a single line at a time, ensuring
            // we don't read past the end of the file
            while ((str = sr.ReadLine()) != null)
            {
                variablesToSave.LevelScoresDictToSave[key] = int.Parse(str);
                key++;
            }

            sr.Close();
            file.Close();

            return str;
        }

        else
        {
            return null;
        }

#else

            return null;

#endif
    }

    /// <summary>
    /// Writes each dictionary entry on a single line.
    /// The saved document should have as many lines
    /// as there are dictionary entries and assigns a dictionary key to a 
    /// corresponding line in the file e.g. (LevelScoreDict[Level_1] = first line written to the file
    /// </summary>
    /// <param name="str"></param>
    /// <param name="filename"></param>
    public void WriteDictToFile(string filename, VariablesForSaving variablesToSave)
    {

#if !WEB_BUILD

        // path to the specified file name
        string path = PathForDocumentFile(filename);

        // using file stream to actually find and open/create the file which we are
        // writing to.
        FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write);

        // stream writer allows us to actually read
        // the file
        StreamWriter sw = new StreamWriter(file);

        // temp string to write each entry to the saved location
        string str;

        // while foreach is bad to use, the dict we use it not that big
        // and this ensures we don't grab an empty entry and start at level 1, not level 0
        foreach (int key in variablesToSave.LevelScoresDictToSave.Keys)
        {
            str = variablesToSave.LevelScoresDictToSave[key].ToString();
            sw.WriteLine(str);
        }

        sw.Close();
        file.Close();

#endif
    }

    #endregion
}
