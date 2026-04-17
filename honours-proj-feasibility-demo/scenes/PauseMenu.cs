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
        Visible = false;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("pause") && !GetTree().Paused)
        {
            pause();
        }

        else if (Input.IsActionJustPressed("pause") && GetTree().Paused)
        {
            resume();
        }
    }
    private void resume()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        GetTree().Paused = false;
        Visible = false;
        animationPlayer.PlayBackwards("blur");
    }

    private void pause()
    {
        Input.MouseMode = Input.MouseModeEnum.Visible;
        GetTree().Paused = true;
        Visible = true;
        animationPlayer.Play("blur");
    }

    private void OnResumePressed()
    {
        resume();
    }

    private void OnRestartPressed()
    {
        GameManager.restartLevel();
        GetTree().Paused = false;
        GetTree().ReloadCurrentScene();
    }

    private void OnQuitPressed()
    {
        GetTree().Quit();
    }
}
