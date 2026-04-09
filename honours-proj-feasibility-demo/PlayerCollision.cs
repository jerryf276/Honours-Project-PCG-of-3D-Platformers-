using Godot;
using System;

public partial class PlayerCollision : Area3D
{
    [Export] PlayerCharacter player;
    JsonWriter jsonWriter;

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
        AreaExited += OnAreaExited;
    }
    private void OnAreaEntered(Area3D area)
    {
        if (area.IsInGroup("killzone"))
        {
            player.respawn();
        }

        if (area.IsInGroup("platform"))
        {
           // GD.Print("Player is on" + area.Name);
            player.setPlatformJumpedOn(area.Name);
            Node3D platform = area.GetParent<Node3D>();

            if (jsonWriter == null)
            {
                jsonWriter = GetNode<JsonWriter>("../../JsonWriter");
            }
            jsonWriter.determinePlatformBeforeAfter(platform.GlobalPosition);
            player.setCurrentPlatformPosition(platform.GlobalPosition);

            
        }

        if (area.IsInGroup("spikes"))
        {
           // if (player.IsOnFloor())
          //  {
                GD.Print("Health lost!");
                player.setOnSpike(true);
            if (!player.isCurrentlyAttacked())
                {
                    player.attackedBySpike();
                
                }
             
            
          //  }
        }

        //if (area.IsInGroup("checkpoint"))
        //{
        //    player.setRespawnPosition();
        //}
    }

    private void OnAreaExited(Area3D area)
    {
        if (area.IsInGroup("spikes"))
        {
            //set 
            player.setOnSpike(false);
        }
    }

}
