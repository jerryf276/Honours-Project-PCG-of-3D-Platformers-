using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

public partial class JsonWriter : Node
{
	//List<string> data = new List<string>();
	Godot.Collections.Array data = new Godot.Collections.Array();

	Godot.Collections.Array levelData = new Godot.Collections.Array();

	List<List<Vector3>> platformPositions = new List<List<Vector3>> { };
	List<List<String>> platformInformation = new List<List<String>> { };
	List<List<String>> levelInformation = new List<List<String>> { };
	List<string> generatedLevelData = new List<string> { };
	List<String> chosenAttributes = new List<String> { };

	string platformBeforeCurrent;
	string platformAfterCurrent;
	


	bool jsonDone = false;

	GameTimer gameTimer;

	public override void _Ready()
	{
		//gameTimer = GetNode<GameTimer>("../TestLevel/GameTimer");
	}

	public override void _Process(double delta)
	{
		//if (jsonDone == false)
		//{
		//	addData("Hello! ");
		//	addData("Died at: blah blah blah blah");
		//	addToJson();
		//	addData("Died at: blah blah blah blahhhhhhh");
		//	addToJson();
		//	jsonDone = true;
		//}
	}
	public void addData(string dataToAdd)
	{
		if (data == null)
			data = new Godot.Collections.Array(); // safety check

		data.Add(dataToAdd);
	}

	public void addLevelData(string dataToAdd)
	{
		if (gameTimer == null) 
		{
			gameTimer = GetNode<GameTimer>("../TestLevel/GameTimer");
		}

		//dataToAdd = dataToAdd + " [Current time: " + gameTimer.displayCurrentTime() + "]";

		string stringToDisplay = "[Current time: " + gameTimer.displayCurrentTime() + "] " + dataToAdd;
		if (levelData == null)
		{
			levelData = new Godot.Collections.Array();
		}

	  //  levelData.Add(dataToAdd);
		levelData.Add(stringToDisplay);
		outputDataToJson();
	}
	
	public void addPlatform(Vector3 position, string platformType, int currentSection)
	{
		if (platformPositions.Count <= (currentSection))
		{
            platformPositions.Add(new List<Vector3>());
        }

		if (platformInformation.Count <= (currentSection))
		{
            platformInformation.Add(new List<string>());
        }
		//if (platformPositions[currentSection - 1].Capacity <= 0)
		//{
  //          platformPositions.Add(new List<Vector3>());
  //      }

		//if (platformInformation[currentSection - 1].Capacity <= 0)
		//{
		//	platformInformation.Add(new List<string>());
		//}

		platformPositions[currentSection - 1].Add(position);
        //platformInformation[currentSection - 1].Add("Section " + currentSection + ", Platform Type:" +  platformType + ", Position: " + position);
        platformInformation[currentSection - 1].Add("Platform Type:" + platformType + ", Position: " + position);
    }

	public void addLevelPartToJson(Vector3 position, string Type, int currentSection)
	{
		if (levelInformation.Count <= (currentSection))
		{
            levelInformation.Add(new List<string>());
        }

		if (position == new Vector3(0, -100, 0))
		{
            levelInformation[currentSection - 1].Add(Type);
        }

		else
		{
            levelInformation[currentSection - 1].Add(Type + " Position: " + position);
        }
			//levelInformation[currentSection - 1].Add(Type);
		//if (levelInformation[currentSection - 1].Capacity <= 0)
		//{
		//	levelInformation.Add(new List<string>());
		//}
	}

	private void outputDataToJson()
	{
		using var jsonLine = FileAccess.Open("res://LevelResults.json", FileAccess.ModeFlags.Write);

		GD.Print("Written to json!");

		//if (levelData == null)
		//{
		//    GD.PrintErr("Data is null! Cannot write JSON.");
		//    return;
		//}
		//if (jsonLine == null) 
		//{
		//    GD.PrintErr(")
		//}

		foreach (string item in levelData)
		{
			var jsonString = Json.Stringify(item);
			jsonLine.StoreLine(jsonString);
		}

	}

	public void addDeath()
	{
	   
	}

	public void addSectionNumberToJson(int sectionNumber)
	{
		if (sectionNumber != 1)
		{
            generatedLevelData.Add("");
            generatedLevelData.Add("SECTION NUMBER:" + sectionNumber);
		}

		else
		{
            generatedLevelData.Add("SECTION NUMBER:" + sectionNumber);
        }
	}

	public void addToLevelData(int sectionNumber)
	{
		for (int i = 0; i < platformInformation[sectionNumber - 1].Count; i++)
		{
			generatedLevelData.Add(platformInformation[sectionNumber - 1][i]);
		}
	}

	public void determinePlatformBeforeAfter(Vector3 position)
	{
		for (int i = 0; i < platformPositions.Count; i++)
		{
			for (int j = 0;  j < platformPositions[i].Count; j++)
			{
				if (platformPositions[i][j] == position)
				{
					if (j > 0)
					{
						platformBeforeCurrent = platformInformation[i][j - 1];

						if (j < platformPositions[i].Count - 1)
						{
							//If there is a platform after the current one in the current section.
							platformAfterCurrent = platformInformation[i][j + 1];
						}

						else if ((i + 1) < platformPositions.Count)
						{
							//If the current platform is the last platform in the section, but there is a section that has been spawned afterwards
                                platformAfterCurrent = platformInformation[i + 1][0];
                            
								//platformAfterCurrent = platformInformation[i + 1][0]; 
						}

						else
						{
							//If this is the last platform in the section and there is no platform spawned after it
							platformAfterCurrent = "None";
						}
                    }

					else if (i > 0)
					{
						//If this is the first platform of a section, and the current section is section 2 or later
						platformBeforeCurrent = platformInformation[i - 1][platformPositions[i - 1].Count - 1];
						platformAfterCurrent = platformInformation[i][j + 1];
					}

					else
					{
						//If this is the first platform of a section and the current section is the first section
						platformBeforeCurrent = "None";
						platformAfterCurrent = platformInformation[i][j + 1];
					}


				}
			}
		}
	}



	public string getPlatformBeforeCurrent()
	{
		return platformBeforeCurrent;
	}

	public string getPlatformAfterCurrent()
	{
		return platformAfterCurrent;
	}

	public void addChosenLevelAttributes(string attributeToAdd)
	{
		chosenAttributes.Add(attributeToAdd);
	}

	public void displayChosenLevelAttributes()
	{
		using var jsonLine = FileAccess.Open("res://LevelAttributes.json", FileAccess.ModeFlags.Write);

        foreach (string item in chosenAttributes)
		{
            var jsonString = Json.Stringify(item);
            jsonLine.StoreLine(jsonString);
        }
    }


    public void outputGeneratedLevelToJson()
    {
        using var jsonLine = FileAccess.Open("res://GeneratedLevel.json", FileAccess.ModeFlags.Write);

		for (int i = 0; i < levelInformation.Count; i++)
		{
            if (i != 0)
            {
                var jsonStringGap = Json.Stringify("");
                jsonLine.StoreLine(jsonStringGap);
            }
            var jsonString = Json.Stringify("SECTION " + (i + 1) + ":");
            jsonLine.StoreLine(jsonString);
            // var jsonString = Json.Stringify("SECTION " + (i + 1) + ":");
            foreach (string item in levelInformation[i])
            {
                jsonString = Json.Stringify(item);
                jsonLine.StoreLine(jsonString);
            }
        }

    }

}
