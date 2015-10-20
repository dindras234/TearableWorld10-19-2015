/*
	FILE: StatisticManager
	
	DESCRIPTION:
		This file is used to keep track of the player's statistics in game for achievements, saving, and loading game sessions.
	
	AUTHORS: Tearable World Team (Crumpled Up Games' Engineers)
		-> John Crocker, Justin Telmo, Tom Dubiner ... (ADD YOUR NAME HERE!!!!)
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;


/***************************** IMPORTANT *****************************
	Inner class of variables to save. Dont get confused with StatisticManager's fields that are named the same thing!!!
*/
[System.Serializable]
public class VariablesForSaving{
	// This dictionary is used to keep track of the highest score for each level the player has achieved.
	public Dictionary<int, int> LevelScoresDictToSave;

	// The dictionary that's used to save the name of the high score user with a level.
	public Dictionary<int, string> NameToLevelDictToSave;

	public VariablesForSaving(){
		LevelScoresDictToSave = new Dictionary<int, int>();
		NameToLevelDictToSave = new Dictionary<int, string>();
	}

	public VariablesForSaving(SerializationInfo info, StreamingContext context){
		foreach(int key in LevelScoresDictToSave.Keys){
			LevelScoresDictToSave[key] = (int)info.GetValue("LevelScoresDictToSave[key]", typeof(int));
		}

		foreach(int key in NameToLevelDictToSave.Keys){
			NameToLevelDictToSave[key] = (string)info.GetValue("NameToLevelDictToSave[key]", typeof(string));
		}
	}

	public void GetLevelScoreObjectData(SerializationInfo info, StreamingContext context){
		foreach(int key in LevelScoresDictToSave.Keys){
			info.AddValue("LevelScoresDictToSave[key]", typeof(int));
		}
	}

	public void GetNameObjectData(SerializationInfo info, StreamingContext context){
		foreach(int key in NameToLevelDictToSave.Keys){
			info.AddValue("NameToLevelDictToSave[key]", typeof(string));
		}
	}
}
/****************** END INNER SERIALIZABLE CLASS *********************/
[System.Serializable]

public class StatisticManager : MonoBehaviour{
	#region Fields

	// This represents the player's saved game, where the needed data is stored for launching a reloaded game session.
	//private SavedData savedGame;
	
	// Save the total number of folds, tears, and rotates across a player's playtime.
	public int totalNumFolds, totalNumRotates, totalNumTears;
	
	// The number levels completed represents the current number of levels the player has compeleted.
	private int numLevelsCompleted;

	// This is used to flag whether or not a player is currently trying to resume a saved game session.
	private bool resumeSession;

	// This represents the total number of levels Tearable world has to offer the player.
	private const int TOTAL_NUM_LEVELS = 25;

	// This represents current high score for the only level above
	//	TODO: This will change once the player has gained points in game, updating the levelScores dictionary accordingly.
	private const int LEVEL_HIGH_SCORE = 0;

	// The name of the file storing data to be saved/loaded.
	private const string HIGH_SCORE_FILE_NAME = "savedScoreData";

	// File location of names associated with high scores to be saved/loaded.
	private const string USER_NAME_FILE_NAME = "savedUsernameData";

	// NOT TO BE CONFUSED WITH THE INNER CLASS
	//	This dictionary is used to keep track of the highest score for each level the player has achieved.
	public Dictionary<int, int> LevelScoresDict;

	// NOT TO BE CONFUSED WITH THE INNER CLASS
	// Keeps track of user names associated with high scores on levels.
	public Dictionary<int, string> NameToLevelDict;
	#endregion

	#region Methods
	public string GetMobileDocumentsPath(){
		/*
			Your game has read+write access to /var/mobile/Applications/XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX/Documents 
			Application.dataPath returns			  
			/var/mobile/Applications/XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX/myappname.app/Data 
			Strip "/Data" from path
		*/
		string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
		// Strip application name .
		path = path.Substring(0, path.LastIndexOf('/'));
		return path + "/Documents";
	}

	// Use this for initialization.
	private void Start(){
		LevelScoresDict = new Dictionary<int, int>();
		NameToLevelDict = new Dictionary<int, string>();
	}

	// Update is called once per frame.
	private void Update(){}

	// Saves the game and all stats needed for next relaunch.
	/* Input:
		'currentLevel': The current level being saved.
		'name': The name of the player who had played this level.
		'score': The score to be saved.
	*/
	public void SaveGame(int currentLevel, string name, int score){
		VariablesForSaving variablesToSave = new VariablesForSaving();

		if(NameToLevelDict.ContainsKey(currentLevel)){
			NameToLevelDict.Remove(currentLevel);
			LevelScoresDict.Remove(currentLevel);
		}

		NameToLevelDict.Add(currentLevel, name);
		LevelScoresDict.Add(currentLevel, score);
		
		variablesToSave.LevelScoresDictToSave = LevelScoresDict;
		variablesToSave.NameToLevelDictToSave = NameToLevelDict;

		WriteHighScoreDictToFile(HIGH_SCORE_FILE_NAME, variablesToSave);
		WriteNameDictToFile(USER_NAME_FILE_NAME, variablesToSave);

		/*Debug.Log(
				"Player: " + name +
				"\nLevel: " + currentLevel +
				"\nScore: " + score);*/
		LoadSavedGames();
	}

	// Loads the save game for the player.
	public void LoadSavedGames(){
		VariablesForSaving variablesToSave = new VariablesForSaving();

		ReadHighScoreDictFromFile(HIGH_SCORE_FILE_NAME, variablesToSave);
		ReadNameDictFromFile(USER_NAME_FILE_NAME, variablesToSave);

		LevelScoresDict = variablesToSave.LevelScoresDictToSave;
		NameToLevelDict = variablesToSave.NameToLevelDictToSave;
	}

	// Returns a string with the path to the specified filename given as the input parameter. The save path differs
	//	depending on which platform the user is using.
	public string PathForDocumentFile(string filename){
		if(Application.platform == RuntimePlatform.IPhonePlayer){
			string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
			path = path.Substring(0, path.LastIndexOf('/'));
			return Path.Combine(Path.Combine(path, "Documents"), filename);
		}
		else if(Application.platform == RuntimePlatform.Android){
			string path = Application.persistentDataPath;
			path = path.Substring(0, path.LastIndexOf('/'));
			return Path.Combine(path, filename);
		}
		else{
			string path = Application.dataPath;
			path = path.Substring(0, path.LastIndexOf('/'));
			return Path.Combine(path, filename);
		}
	}
	
	/* For High Scores:
		Iterates through the saved file specified from the input paramater one line at a time. The saved document
			should have as many lines as there are dictionary entries and assigns a dictionary key to a corresponding
			line in the file e.g. (LevelScoreDict[Level_1] = first line read from the file.
	*/
	public string ReadHighScoreDictFromFile(string filename, VariablesForSaving variablesToSave){
		#if !WEB_BUILD
		// Path to the specified file name.
		string path = PathForDocumentFile(filename);

		// check to see if the path exists, otherwise return null
		if(File.Exists(path)){
			// Using file stream to actually find and open/create the file which we are reading from.
			FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);

			// Stream writer allows us to actually read the file.
			StreamReader sr = new StreamReader(file);

			// Temporary string to read from.
			string str = null;

			// Dictionary starts at entry 1, corresponding to level number.
			int key = 1;

			// Read a single line at a time, ensuring we don't read past the end of the file.
			while((str = sr.ReadLine()) != null){
				variablesToSave.LevelScoresDictToSave[key] = int.Parse(str);
				key++;
			}

			sr.Close();
			file.Close();

			return str;
		}
		else{
			return null;
		}
		#else
			return null;
		#endif
	}
	
	/* For high score names:
		Iterates through the saved file specified from the input paramater one line at a time. The saved document
			should have as many lines as there are dictionary entries and assigns a dictionary key to a corresponding
			line in the file e.g. (LevelScoreDict[Level_1] = first line read from the file.
	*/
	public string ReadNameDictFromFile(string filename, VariablesForSaving variablesToSave){
		#if !WEB_BUILD
		// Path to the specified file name.
		string path = PathForDocumentFile(filename);

		// Check to see if the path exists, otherwise return null.
		if(File.Exists(path)){
			// Using file stream to actually find and open/create the file which we are reading from.
			FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);

			// Stream writer allows us to actually read the file.
			StreamReader sr = new StreamReader(file);

			// Temporary string to read from
			string str = null;

			// Dictionary starts at entry 1, corresponding to level number
			int key = 1;

			// Read a single line at a time, ensuring we don't read past the end of the file.
			while((str = sr.ReadLine()) != null){
				variablesToSave.NameToLevelDictToSave[key] = str;
				key++;
			}

			sr.Close();
			file.Close();

			return str;
		}
		else{
			return null;
		}
		#else
			return null;
		#endif
	}
	
	/* For high scores:
		Writes each dictionary entry on a single line. The saved document should have as many lines as there
			are dictionary entries and assigns a dictionary key to a corresponding line in the file e.g.
			(LevelScoreDict[Level_1] = first line written to the file.
	*/
	public void WriteHighScoreDictToFile(string filename, VariablesForSaving variablesToSave){
		#if !WEB_BUILD
		// Path to the specified file name.
		string path = PathForDocumentFile(filename);

		// Using file stream to actually find and open/create the file which we are writing to.
		FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write);

		// Stream writer allows us to actually read the file.
		StreamWriter sw = new StreamWriter(file);

		// Temp string to write each entry to the saved location.
		string str;

		// While foreach is bad to use, the dict we use it not that big and this ensures we don't grab an empty
		//	entry and start at level 1, not level 0.
		foreach(int key in variablesToSave.LevelScoresDictToSave.Keys){
			str = variablesToSave.LevelScoresDictToSave[key].ToString();
			sw.WriteLine(str);
		}

		sw.Close();
		file.Close();
		#endif
	}

	/* For user names.
		Writes each dictionary entry on a single line. The saved document should have as many lines as there are
			dictionary entries and assigns a dictionary key to a corresponding line in the file e.g.
			(LevelScoreDict[Level_1] = first line written to the file.
	*/
	public void WriteNameDictToFile(string filename, VariablesForSaving variablesToSave){
		#if !WEB_BUILD
		// Path to the specified file name.
		string path = PathForDocumentFile(filename);

		// Using file stream to actually find and open/create the file which we are writing to.
		FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write);

		// Stream writer allows us to actually read the file.
		StreamWriter sw = new StreamWriter(file);

		// Temp string to write each entry to the saved location.
		string str;

		// While foreach is bad to use, the dict we use it not that big and this ensures we don't grab an empty
		//	entry and start at level 1, not level 0.
		foreach(int key in variablesToSave.LevelScoresDictToSave.Keys){
			str = variablesToSave.NameToLevelDictToSave[key].ToString();
			sw.WriteLine(str);
		}

		sw.Close();
		file.Close();
		#endif
	}
	#endregion
}
