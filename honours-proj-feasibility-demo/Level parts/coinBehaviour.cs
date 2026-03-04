using Godot;
using System;

public partial class coinBehaviour : Area3D
{
    bool coinCollected = false;

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
        //AreaExited += OnAreaExited;
    }
    public void collect()
    {

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
                    player.addCoinCount(1);
                    player.addScore(100);
                    GameManager.updateCoinText(player.getCoinCount());
                    GetParent().QueueFree();
                }
            }
        }
    }
}
