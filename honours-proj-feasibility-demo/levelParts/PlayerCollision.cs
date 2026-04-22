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
            //Player dies due to falling off
            player.respawn();
        }

        if (area.IsInGroup("platform"))
        {
            //If player collides with platform, the platform the player is currently on will be documented
            player.setPlatformJumpedOn(area.Name);
            Node3D platform = area.GetParent<Node3D>();

            if (jsonWriter == null)
            {
                jsonWriter = GetNode<JsonWriter>("../../JsonWriter");
            }

            //Platform player was on before the current one will be documented
            jsonWriter.determinePlatformBeforeAfter(platform.GlobalPosition);

            //Platform player was on after the current one will be documented
            player.setCurrentPlatformPosition(platform.GlobalPosition);

            
        }

        if (area.IsInGroup("spikes"))
        {
            //When player collides with spikes, their health will be lost
            GD.Print("Health lost!");
            player.setOnSpike(true);

            if (!player.isCurrentlyAttacked())
            {
                player.attackedBySpike();
                
            }
        }
    }

    private void OnAreaExited(Area3D area)
    {
        //Exiting from spike collision area
        if (area.IsInGroup("spikes"))
        {
            player.setOnSpike(false);
        }
    }

}
