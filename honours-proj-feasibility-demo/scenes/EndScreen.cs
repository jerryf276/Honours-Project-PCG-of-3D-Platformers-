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
    [Export] Label rankText;
    [Export] Label deathText;
    //PlayerCharacter player;
    public override void _Ready()
    {
        restartButton.Pressed += OnRestartEntered;
        quitButton.Pressed += OnQuitEntered;
        Visible = false;

        //player = GetTree().Root.GetNode<PlayerCharacter>("Game/PlayerCharacter") as PlayerCharacter;
    }
    public override void _Process(double delta)
    {
       // player = GetTree().Root.GetNode<PlayerCharacter>("Game/PlayerCharacter") as PlayerCharacter;
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

    public void LevelFinished()
    {
        PlayerCharacter player;
        player = GetTree().Root.GetNode<PlayerCharacter>("Game/PlayerCharacter") as PlayerCharacter;
        Input.MouseMode = Input.MouseModeEnum.Visible;
        GetTree().Paused = true;
        Visible = true;
        scoreText.Text = "Score: " + player.getScore();
        deathText.Text = "Deaths: " + player.getDeathCount();
       // animationPlayer.Play("blur");
    }

}
