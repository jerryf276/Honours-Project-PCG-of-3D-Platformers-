using Godot;
using System;

public partial class PauseMenu : Control
{
    [Export] AnimationPlayer animationPlayer;
    [Export] Button resumeButton;
    [Export] Button restartButton;
    [Export] Button quitButton;

    public override void _Ready()
    {
        resumeButton.Pressed += OnResumePressed;
        restartButton.Pressed += OnRestartPressed;
        quitButton.Pressed += OnQuitPressed;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("cancel") && !GetTree().Paused)
        {
            pause();
        }

        else if (Input.IsActionJustPressed("cancel") && GetTree().Paused)
        {
            resume();
        }
    }
    private void resume()
    {
        GetTree().Paused = false;
        animationPlayer.PlayBackwards("blur");
    }

    private void pause()
    {
        GetTree().Paused = false;
        animationPlayer.Play("blur");
    }

    private void OnResumePressed()
    {
        resume();
    }

    private void OnRestartPressed()
    {
        GetTree().ReloadCurrentScene();
    }

    private void OnQuitPressed()
    {
        GetTree().Quit();
    }
}
