using Godot;
using System;

public partial class EndScreen : Control
{
    //[Export] AnimationPlayer animationPlayer;

    [Export] AnimationPlayer animationPlayer;
    [Export] Button restartButton;
    [Export] Button quitButton;
    [Export] Label scoreText;
    [Export] Label timeText;
    [Export] Label finalScoreText;
    [Export] Label coinsText;
    [Export] Label coinBonusText;
    [Export] Label deathText;
    //PlayerCharacter player;
    public override void _Ready()
    {
        restartButton.Pressed += OnRestartEntered;
        quitButton.Pressed += OnQuitEntered;
        Visible = false;
    }
    public override void _Process(double delta)
    {

    }

    private void OnRestartEntered()
    {
        GameManager.restartLevel();
        GetTree().Paused = false;
        GetTree().ReloadCurrentScene();
    }

    private void OnQuitEntered()
    {
        GetTree().Quit();
    }

    private string getTimeText(int minutes, int seconds)
    {
        string mins = "00";
        string secs = "00";
        if (minutes > 0)
        {
            if (minutes <= 9)
            {
                mins = "0" + minutes;
            }

            else
            {
                mins = minutes.ToString();
            }
        }
        else
        {
            mins = "00";
        }

        if (seconds > 0)
        {
            if (seconds <= 9)
            {
                secs = "0" + seconds;
            }

            else
            {
                secs = seconds.ToString();
            }

        }

        return mins + ":" + secs;
    }

    public void LevelFinished()
    {
        PlayerCharacter player;
        player = GetTree().Root.GetNode<PlayerCharacter>("FinalGame/PlayerCharacter") as PlayerCharacter;
        GameTimer gameTimer;
        gameTimer = GetTree().Root.GetNode<GameTimer>("FinalGame/TestLevel/GameTimer");
        Input.MouseMode = Input.MouseModeEnum.Visible;
        GetTree().Paused = true;
        Visible = true;
        scoreText.Text = "Score: " + player.getScore();
        deathText.Text = "Deaths: " + player.getDeathCount();
        timeText.Text = "Time: " + getTimeText(gameTimer.getMinutes(), gameTimer.getSeconds());
        coinsText.Text = "Coins: " + player.getCoinCount();
        coinBonusText.Text = "Coin Bonus: " + (player.getCoinCount() * 10);
        finalScoreText.Text = "Final Score: " + (player.getScore() + (player.getCoinCount() * 10));


        JsonWriter jsonWriter = GetNode<JsonWriter>("../JsonWriter");

        jsonWriter.addLevelData("Level complete!");
        jsonWriter.addLevelData("---POST LEVEL STATS---");
        jsonWriter.addLevelData("SCORE: " +  player.getScore());
        jsonWriter.addLevelData("DEATHS: " + player.getDeathCount());
        jsonWriter.addLevelData("COINS: " + player.getCoinCount());
        jsonWriter.addLevelData("TIME: " + getTimeText(gameTimer.getMinutes(), gameTimer.getSeconds()));
        jsonWriter.addLevelData("COIN BONUS: " + player.getCoinCount() * 10);
        jsonWriter.addLevelData("FINAL SCORE: " + (player.getScore() + (player.getCoinCount() * 10)));
    }

}
