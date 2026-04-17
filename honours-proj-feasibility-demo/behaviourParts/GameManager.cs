using Godot;
using System;

public partial class GameManager : Node
{
    //Text for hud
    Label scoreText;
    Label coinText;
    Label healthText;
    Label timeText;

    //to allow the game manager to exist when using its functions in other scripts (a singleton)
    static GameManager instance;

    //To allow each scene to be loaded when the player hits start
    private Control hud;
    private PlayerCharacter playerCharacter;
    private PauseMenu pauseMenu;
    private EndScreen endScreen;
    private JsonWriter jsonWriter;
    private TestLevel level;
    private CoinPatterns coinPatterns;
    private SpikePatterns spikePatterns;

    public override void _Ready()
    {
        instance = this;
    }


    public static void StartLevel()
    {
        //Loading each scene
        PackedScene hudScene = ResourceLoader.Load<PackedScene>("res://scenes/hud.tscn");
        PackedScene playerScene = ResourceLoader.Load<PackedScene>("res://levelParts/playerCharacter.tscn");
        PackedScene pauseMenuScene = ResourceLoader.Load<PackedScene>("res://scenes/pauseMenu.tscn");
        PackedScene endScreenScene = ResourceLoader.Load<PackedScene>("res://scenes/EndScreen.tscn");
        PackedScene jsonWriterScene = ResourceLoader.Load<PackedScene>("res://behaviourParts/jsonWriter.tscn");
        PackedScene coinPatternScene = ResourceLoader.Load<PackedScene>("res://behaviourParts/coinPatterns.tscn");
        PackedScene spikePatternScene = ResourceLoader.Load<PackedScene>("res://behaviourParts/spikePatterns.tscn");

        //Instantiating each scene into the game
        instance.hud = hudScene.Instantiate<Control>();
        instance.playerCharacter = playerScene.Instantiate<PlayerCharacter>();
        instance.playerCharacter.Position = new Vector3(0, 2, 0);
        instance.pauseMenu = pauseMenuScene.Instantiate<PauseMenu>();
        instance.endScreen = endScreenScene.Instantiate<EndScreen>();
        instance.jsonWriter = jsonWriterScene.Instantiate<JsonWriter>();
        instance.coinPatterns = coinPatternScene.Instantiate<CoinPatterns>();
        instance.spikePatterns = spikePatternScene.Instantiate<SpikePatterns>();

        instance.scoreText = instance.hud.GetNode<Label>("TopHud/MarginContainer/VBoxContainer/HBoxContainer/ScoreText");
        instance.coinText = instance.hud.GetNode<Label>("TopHud/MarginContainer/VBoxContainer/HBoxContainer2/CoinsText");
        instance.timeText = instance.hud.GetNode<Label>("TopHud/MarginContainer/VBoxContainer/HBoxContainer/TimeText");
        instance.healthText = instance.hud.GetNode<Label>("TopHud/MarginContainer/VBoxContainer/HBoxContainer2/HealthText");

        //Adding each scene into the game
        instance.AddChild(instance.hud);
        instance.AddChild(instance.jsonWriter);
        instance.AddChild(instance.playerCharacter);
        instance.AddChild(instance.pauseMenu);
        instance.AddChild(instance.endScreen);
        instance.AddChild(instance.coinPatterns);
        instance.AddChild(instance.spikePatterns);

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
        //Restarts level by wiping it and then reloading it.
        instance.level.QueueFree();
        PackedScene levelToLoad = ResourceLoader.Load<PackedScene>("res://scenes/TestLevel.tscn");
        instance.level = levelToLoad.Instantiate<TestLevel>();
        instance.AddChild(instance.level);

    }

    public static void AddLevel(TestLevel levelToAdd)
    {
        //Used in level modifier to add the level into the game once the start button has been clicked
        instance.level = levelToAdd;
        instance.AddChild(instance.level);
    }

    public static void displayEndScreen()
    {
        //Triggers the end of the game when the player collides with the goal
        instance.endScreen.LevelFinished();
    }
}



