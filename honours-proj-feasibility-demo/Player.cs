using Godot;
using System;

public partial class Player : RigidBody3D
{
    public override void _Ready()
    {
        
    }

    public override void _Process(double delta)
    {
        var input = Input.GetActionStrength("ui_up");
        ApplyCentralForce(Vector3.Forward * input * 1200.0 * delta);
    }
}
