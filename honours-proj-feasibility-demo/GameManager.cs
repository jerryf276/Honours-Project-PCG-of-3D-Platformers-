using Godot;
using System;

public partial class GameManager : Node
{
    Label scoreText;
    Label coinText;
    Label healthText;
    Label timeText;


    static GameManager instance;

    private Control hud;
    private PlayerCharacter playerCharacter;
    private PauseMenu pauseMenu;
    private EndScreen endScreen;
    private JsonWriter jsonWriter;
    private TestLevel level;


    //PackedScene levelToLoad;
    //Node3D level;

    public override void _Ready()
    {
        instance = this;
    }


    public static void StartLevel()
    {
        PackedScene hudScene = ResourceLoader.Load<PackedScene>("res://scenes/hud.tscn");
        PackedScene playerScene = ResourceLoader.Load<PackedScene>("res://player_character.tscn");
        PackedScene pauseMenuScene = ResourceLoader.Load<PackedScene>("res://pauseMenu.tscn");
        PackedScene endScreenScene = ResourceLoader.Load<PackedScene>("res://scenes/EndScreen.tscn");
        PackedScene jsonWriterScene = ResourceLoader.Load<PackedScene>("res://json_writer.tscn");

        instance.hud = hudScene.Instantiate<Control>();
        instance.playerCharacter = playerScene.Instantiate<PlayerCharacter>();
        instance.playerCharacter.Position = new Vector3(0, 2, 0);
        instance.pauseMenu = pauseMenuScene.Instantiate<PauseMenu>();
        instance.endScreen = endScreenScene.Instantiate<EndScreen>();
        instance.jsonWriter = jsonWriterScene.Instantiate<JsonWriter>();

        instance.scoreText = instance.hud.GetNode<Label>("TopHud/MarginContainer/VBoxContainer/HBoxContainer/ScoreText");
        instance.coinText = instance.hud.GetNode<Label>("TopHud/MarginContainer/VBoxContainer/HBoxContainer2/CoinsText");
        instance.timeText = instance.hud.GetNode<Label>("TopHud/MarginContainer/VBoxContainer/HBoxContainer/TimeText");
        instance.healthText = instance.hud.GetNode<Label>("TopHud/MarginContainer/VBoxContainer/HBoxContainer2/HealthText");

        instance.AddChild(instance.hud);
        instance.AddChild(instance.jsonWriter);
        instance.AddChild(instance.playerCharacter);
        instance.AddChild(instance.pauseMenu);
        instance.AddChild(instance.endScreen);

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
        //instance.level.QueueFree()
        //foreach (Node child in instance.level.GetChildren())
        //{
        //    child.QueueFree();
        //}
        // instance.GetTree().Root.QueueFree();

        //    Node3D Level = instance.GetNode<Node3D>("TestLevel");
        // Level.QueueFree();
        PackedScene levelToLoad = ResourceLoader.Load<PackedScene>("res://TestLevel.tscn");
        instance.level.QueueFree();
        instance.level = levelToLoad.Instantiate<TestLevel>();
        instance.AddChild(instance.level);
        //instance.level = instance.levelToLoad.Instantiate<Node3D>();
        //instance.AddChild(instance.level);

    }

    public static void AddLevel(TestLevel levelToAdd)
    {
        instance.level = levelToAdd;
        instance.AddChild(instance.level);
    }

    public static void displayEndScreen()
    {
        //instance.EndScreen
        // instance.EndScreen.
        instance.endScreen.LevelFinished();
    }
}



