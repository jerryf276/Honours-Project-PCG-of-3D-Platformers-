using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class JsonWriter : Node
{
	//List<string> data = new List<string>();
	Godot.Collections.Array data = new Godot.Collections.Array();

	Godot.Collections.Array levelData = new Godot.Collections.Array();

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

	private void outputDataToJson()
	{
		using var jsonLine = FileAccess.Open("res://testFile.json", FileAccess.ModeFlags.Write);

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

	//public void addToJson()
	//{

	//	using var saveFile = FileAccess.Open("user://savegame.json", FileAccess.ModeFlags.ReadWrite);
	//	foreach (string item in data)
	//	{
	//		var jsonString = Json.Stringify(item);
	//		saveFile.StoreLine(jsonString);
	//	}
	//	//if (data == null)
	//	//{
	//	//    GD.PrintErr("Data is null! Cannot write JSON.");
	//	//    return;
	//	//}
	//	//// for (int)
	//	////var dataToUse = new Godot.Collections.Array(data)fix;
	//	//string jsonToWrite = Json.Stringify(data);
	//	//using var file = FileAccess.Open("user:://output.json", FileAccess.ModeFlags.Write);
	//	//file.StoreString(jsonToWrite);
	//}

  
}
