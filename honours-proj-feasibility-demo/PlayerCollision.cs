using Godot;
using System;

public partial class PlayerCollision : Area3D
{
    [Export] PlayerCharacter player;

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
    }
    private void OnAreaEntered(Area3D area)
    {
        if (area.IsInGroup("killzone"))
        {
            player.respawn();
        }

        //if (area.IsInGroup("checkpoint"))
        //{
        //    player.setRespawnPosition();
        //}
    }

}
