using Godot;
using System;

public partial class EndScreen : Control
{
    //[Export] AnimationPlayer animationPlayer;


    [Export] Button restartButton;
    [Export] Button quitButton;
    public override void _Ready()
    {
        restartButton.Pressed += OnRestartEntered;
        quitButton.Pressed += OnQuitEntered;
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

}
