using Godot;
using System;

public partial class Player : RigidBody3D
{
    public override void _Ready()
    {
        
    }

    public override void _Process(double delta)
    {
        var input = Input.GetActionStrength("move_forward");
        ApplyCentralForce(new Vector3(Vector3.Forward.X, Vector3.Forward.Y, Vector3.Forward.Z * input * 1200.0f * (float)delta));
        GD.Print(Position);
    }
}
