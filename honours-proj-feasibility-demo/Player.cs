using Godot;
using System;

public partial class Player : RigidBody3D
{
    public override void _Ready()
    {
        
    }

    public override void _Process(double delta)
    {
        Vector3 input = Vector3.Zero;
        input.X = Input.GetAxis("move_left", "move_right");
        input.Z = Input.GetAxis("move_forward", "move_backward");

        ApplyCentralForce(input * 1200.0f * (float)delta);
    }
}
