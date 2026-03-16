using Godot;
using System;
using System.Collections.Generic;

public partial class JsonWriter : Node
{
    //List<string> data = new List<string>();
    Godot.Collections.Array data = new Godot.Collections.Array();

    bool jsonDone = false;

    public override void _Process(double delta)
    {
        if (jsonDone == false)
        {
            addData("Hello! ");
            addData("Died at: blah blah blah blah");
            addToJson();
            jsonDone = true;
        }
    }
    public void addData(string dataToAdd)
    {
        if (data == null)
            data = new Godot.Collections.Array(); // safety check

        data.Add(dataToAdd);
    }

    public void addToJson()
    {

        using var saveFile = FileAccess.Open("user://savegame.json", FileAccess.ModeFlags.Write);
        foreach (string item in data)
        {
            var jsonString = Json.Stringify(item);
            saveFile.StoreLine(jsonString);
        }
        //if (data == null)
        //{
        //    GD.PrintErr("Data is null! Cannot write JSON.");
        //    return;
        //}
        //// for (int)
        ////var dataToUse = new Godot.Collections.Array(data)fix;
        //string jsonToWrite = Json.Stringify(data);
        //using var file = FileAccess.Open("user:://output.json", FileAccess.ModeFlags.Write);
        //file.StoreString(jsonToWrite);
    }
}
