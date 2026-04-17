using Godot;
using System;

public partial class CoinBehaviour : Area3D
{
    bool coinCollected = false;

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
    }

    private void OnAreaEntered(Area3D area)
    {
        if (coinCollected)
        {
            return;
        }

        if (area.IsInGroup("player"))
        {
            var playerArea = GetTree().GetFirstNodeInGroup("player");

            if (playerArea != null)
            {
                PlayerCharacter player = playerArea.GetParent() as PlayerCharacter;
                if (player != null)
                {
                    //Adds coin score to player's score and kills the coin
                    player.addCoinCount(1);
                    player.addScore(100);
                    GameManager.updateCoinText(player.getCoinCount());
                    GetParent().QueueFree();
                }
            }
        }
    }
}
