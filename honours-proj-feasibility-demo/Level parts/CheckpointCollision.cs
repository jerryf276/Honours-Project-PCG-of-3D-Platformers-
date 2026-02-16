using Godot;
using System;

public partial class CheckpointCollision : Area3D
{
    //PlayerCharacter player;
    bool checkpointReached = false;
    public override void _Ready()

    {
        //player = GetTree().Root.GetNode<PlayerCharacter>("../../../../../../PlayerCharacter");
        AreaEntered += OnAreaEntered;
    }

    private void OnAreaEntered(Area3D area)
    {
        if (checkpointReached == false)
        {
            if (area.IsInGroup("player"))
            {
                var playerArea = GetTree().GetFirstNodeInGroup("player");

                if (playerArea != null)
                {
                    PlayerCharacter player = playerArea.GetParent() as PlayerCharacter;
                    if (player != null)
                    {
                        player.setRespawnPosition(new Vector3(GlobalPosition.X, GlobalPosition.Y + 3, GlobalPosition.Z));
                        GD.Print("Checkpoint!");
                        checkpointReached = true;
                    }
                }

            }
        }
    }


}
