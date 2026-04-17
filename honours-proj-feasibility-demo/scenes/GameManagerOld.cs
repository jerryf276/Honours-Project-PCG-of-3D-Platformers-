using Godot;
using System;
using static System.Formats.Asn1.AsnWriter;

public partial class GameManagerOld : Node
{
    [Export] Control inGameHUD;
    [Export] EndScreen EndScreen;

    Label scoreText;
    Label coinText;
    Label healthText;
    Label timeText;

    static GameManagerOld instance;
    PackedScene levelToLoad;
    Node3D level;

    public override void _Ready()
    {
        scoreText = inGameHUD.GetNode<Label>("TopHud/MarginContainer/VBoxContainer/HBoxContainer/ScoreText");
        coinText = inGameHUD.GetNode<Label>("TopHud/MarginContainer/VBoxContainer/HBoxContainer2/CoinsText");
        healthText = inGameHUD.GetNode<Label>("TopHud/MarginContainer/VBoxContainer/HBoxContainer2/HealthText");
        timeText = inGameHUD.GetNode<Label>("TopHud/MarginContainer/VBoxContainer/HBoxContainer/TimeText");

        instance = this;

        levelToLoad = ResourceLoader.Load<PackedScene>("res://TestLevel.tscn");
        level = levelToLoad.Instantiate<Node3D>();
        AddChild(level);
    }

    public override void _Process(double delta)
    {
        
    }
    
    public static void updateScoreText(int score)
    {
        instance.scoreText.Text = "Score: " + score;
    }

    public static void updateCoinText(int coinCount)
    {
        instance.coinText.Text = "Coins: " + coinCount;
    }

    public static void updateHealthText(int healthCount)
    {
        instance.healthText.Text = "Health: " + healthCount;
    }

    public static void updateTimeText(string text)
    {
        instance.timeText.Text = text;
    }


    public static void restartLevel()
    {
        instance.level.QueueFree();
        instance.level = instance.levelToLoad.Instantiate<Node3D>();
        instance.AddChild(instance.level);

    }

    public static void startLevel()
    {
        instance.levelToLoad = ResourceLoader.Load<PackedScene>("res://scenes/TestLevel.tscn");
        instance.level = instance.levelToLoad.Instantiate<Node3D>();
        instance.AddChild(instance.level);
    }

    public static void displayEndScreen()
    {
        instance.EndScreen.LevelFinished();
    }

}
