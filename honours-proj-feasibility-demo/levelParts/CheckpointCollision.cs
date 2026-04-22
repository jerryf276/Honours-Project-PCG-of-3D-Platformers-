using Godot;
using System;

public partial class CheckpointCollision : Area3D
{
    //PlayerCharacter player;
    bool checkpointReached = false;
    public override void _Ready()

    {
        AreaEntered += OnAreaEntered;
    }

    private void OnAreaEntered(Area3D area)
    {
        if (checkpointReached == false)
        {
            //Player reaching checkpoint
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
                        //Player earns checkpoint (gains 2000 points)
                        player.addScore(2000);
                        checkpointReached = true;
                    }

                    TestLevel level = GetTree().Root.GetNode<TestLevel>("FinalGame/TestLevel") as TestLevel;

                    if (level != null)
                    {
                        level.setCheckpointReached(checkpointReached);
                    }

                }

            }
        }
    }


}
