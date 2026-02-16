using Godot;
using System;

public partial class CheckpointCollision : Area3D
{
    //PlayerCharacter player;

    public override void _Ready()

    {
        //player = GetTree().Root.GetNode<PlayerCharacter>("../../../../../../PlayerCharacter");
        AreaEntered += OnAreaEntered;
    }

    private void OnAreaEntered(Area3D area)
    {
       // var player;
        if (area.IsInGroup("player")) {
            //player = player = GetTree().GetFirstNodeInGroup<PlayerCharacteri nee>("player");
            var playerArea = GetTree().GetFirstNodeInGroup("player");

            if (playerArea != null)
            {
                PlayerCharacter player = playerArea.GetParent() as PlayerCharacter;
                if (player != null)
                {
                    player.setRespawnPosition(new Vector3(GlobalPosition.X, GlobalPosition.Y + 3, GlobalPosition.Z));
                }
            }
            //if (player is PlayerCharacter)
            //{
           
           // }
          //  player.setRespawnPosition(new Vector3(Position.X, Position.Y + 3, Position.Z));
        }
    }


}
