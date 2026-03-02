using Godot;
using System;

public partial class GameManager : Node
{
    [Export] Control inGameHUD;

    Label scoreText;
    Label coinText;
    Label healthText;
    Label timeText;


    public override void _Ready()
    {
        scoreText = inGameHUD.GetNode<Label>("TopHud/MarginContainer/VBoxContainer/HBoxContainer/ScoreText");
        coinText = inGameHUD.GetNode<Label>("TopHud/MarginContainer/VBoxContainer/HBoxContainer2/CoinsText");
        healthText = inGameHUD.GetNode<Label>("TopHud/MarginContainer/VBoxContainer/HBoxContainer2/HealthText");
        timeText = inGameHUD.GetNode<Label>("TopHud/MarginContainer/VBoxContainer/HBoxContainer/TimeText");
    }

    public override void _Process(double delta)
    {
        
    }
    
    private void updateScoreText(int score)
    {

    }

    private void updateCoinText(int coinCount)
    {

    }

    private void updateHealthText(int healthCount)
    {

    }
}
