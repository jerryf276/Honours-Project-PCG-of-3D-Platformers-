using Godot;
using System;

public partial class healthOrbBehaviour : Area3D
{
    bool healthTaken = false;

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
    }
    private void OnAreaEntered(Area3D area)
    {
        if (healthTaken)
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
                    //Restore health here
                    player.healPlayer();
                    GameManager.updateHealthText(player.getHealth());
                    GetParent().QueueFree();
                }
            }
        }
    }
}
